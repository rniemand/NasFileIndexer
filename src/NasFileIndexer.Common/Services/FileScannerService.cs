using System.Text.RegularExpressions;
using NasFileIndexer.Common.Models;
using NasFileIndexer.Common.Providers;
using NasFileIndexer.Common.Repo;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Extensions;
using Rn.NetCore.Common.Factories;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Wrappers;

namespace NasFileIndexer.Common.Services;

public interface IFileScannerService
{
  Task TickAsync(CancellationToken stoppingToken);
}

public class FileScannerService : IFileScannerService
{
  private readonly ILoggerAdapter<FileScannerService> _logger;
  private readonly IDateTimeAbstraction _dateTime;
  private readonly IIOFactory _ioFactory;
  private readonly IFileRepo _fileRepo;
  private readonly NasFileIndexerConfig _config;
  private DateTime _nextScanTime;

  public FileScannerService(ILoggerAdapter<FileScannerService> logger,
    IDateTimeAbstraction dateTime,
    IIOFactory ioFactory,
    IConfigProvider configProvider,
    IFileRepo fileRepo)
  {
    _logger = logger;
    _dateTime = dateTime;
    _ioFactory = ioFactory;
    _fileRepo = fileRepo;

    _config = configProvider.GetConfig();
    _nextScanTime = _dateTime.Now.AddSeconds(1);

#if DEBUG
    _nextScanTime = _dateTime.MinValue;
#endif
  }

  public async Task TickAsync(CancellationToken stoppingToken)
  {
    if (_dateTime.Now < _nextScanTime)
      return;

    _logger.LogInformation("Starting to index {count} source(s)",
      _config.ScanPaths.Length);

    // Ensure that we start with a clean "Files" DB table
    await _fileRepo.TruncateTableAsync();

    Parallel.ForEach(_config.ScanPaths, async (path) =>
      await ScanDirRecursive(path, 1, stoppingToken));

    _logger.LogInformation("Indexing completed");
    _nextScanTime = _dateTime.Now.AddHours(24);
  }


  // Internal methods
  private async Task ScanDirRecursive(string path, int depth, CancellationToken stoppingToken)
  {
    if (!CanScanDirectory(path, depth, stoppingToken))
      return;

    try
    {
      _logger.LogInformation("Scanning directory depth {depth}: {path}", depth, path);
      IDirectoryInfo directory = _ioFactory.GetDirectoryInfo(path);

      foreach (IDirectoryInfo subDirInfo in directory.GetDirectories())
        await ScanDirRecursive(subDirInfo.FullName, depth + 1, stoppingToken);

      var files = new List<FileEntity>();
      files.AddRange(directory.GetFiles().Select(MapFileEntry));

      await SaveResultsAsync(files, stoppingToken);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to index directory '{path}': {error}", path, ex.HumanStackTrace());
    }
  }

  private async Task SaveResultsAsync(List<FileEntity> files, CancellationToken stoppingToken)
  {
    if (stoppingToken.IsCancellationRequested)
      return;

    _logger.LogDebug("Saving {count} file(s) to the DB", files.Count);
    await _fileRepo.AddManyAsync(files);
  }

  private static string[] ExtractPathParts(string filePath)
  {
    var baseDir = (Path.GetDirectoryName(filePath) ?? "")
      .Replace("\\", "/")
      .Replace("//", "/");

    var pathParts = new List<string>();

    while (baseDir.Contains("/") || baseDir.Contains("\\"))
    {
      var currentDir = Path.GetFileName(baseDir);

      if (!string.IsNullOrEmpty(currentDir))
        pathParts.Add(currentDir);

      baseDir = Path.GetDirectoryName(baseDir) ?? string.Empty;
    }

    pathParts.Reverse();
    return pathParts.ToArray();
  }

  private static FileEntity MapFileEntry(IFileInfo fileInfo)
  {
    var pathParts = ExtractPathParts(fileInfo.FullName);

    return new FileEntity
    {
      CreationTimeUtc = fileInfo.CreationTimeUtc,
      Extension = fileInfo.Extension,
      FileName = fileInfo.Name,
      FileSize = fileInfo.Length,
      FileSizeKb = GetFileSizeKb(fileInfo),
      FileSizeMb = GetFileSizeMb(fileInfo),
      FileSizeGb = GetFileSizeGb(fileInfo),
      LastAccessTimeUtc = fileInfo.LastAccessTimeUtc,
      LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
      PathSegment01 = pathParts.Length >= 1 ? pathParts[0] : null,
      PathSegment02 = pathParts.Length >= 2 ? pathParts[1] : null,
      PathSegment03 = pathParts.Length >= 3 ? pathParts[2] : null,
      PathSegment04 = pathParts.Length >= 4 ? pathParts[3] : null,
      PathSegment05 = pathParts.Length >= 5 ? pathParts[4] : null,
      PathSegment06 = pathParts.Length >= 6 ? pathParts[5] : null,
      PathSegment07 = pathParts.Length >= 7 ? pathParts[6] : null,
      PathSegment08 = pathParts.Length >= 8 ? pathParts[7] : null,
      PathSegment09 = pathParts.Length >= 9 ? pathParts[8] : null,
      PathSegment10 = pathParts.Length >= 10 ? pathParts[9] : null,
      PathSegment11 = pathParts.Length >= 11 ? pathParts[10] : null,
      PathSegment12 = pathParts.Length >= 12 ? pathParts[11] : null,
      PathSegment13 = pathParts.Length >= 13 ? pathParts[12] : null,
      PathSegment14 = pathParts.Length >= 14 ? pathParts[13] : null,
      PathSegment15 = pathParts.Length >= 15 ? pathParts[14] : null
    };
  }

  private bool CanScanDirectory(string path, int depth, CancellationToken stoppingToken)
  {
    // Check to see if a STOP was requested
    if (stoppingToken.IsCancellationRequested)
      return false;

    // Check to see if we have hit the max folder depth
    if (depth > _config.MaxScanDepth)
      return false;

    // Check to see if we have any configured skip paths
    if (_config.SkipPaths.Length > 0)
    {
      if (_config.SkipPaths.Any(x => x.IgnoreCaseEquals(path)))
      {
        _logger.LogTrace("Skipping configured path: {path}", path);
        return false;
      }
    }

    // Run RegEx patterns to see if we need to exclude this file
    if (_config.SkipPathExpressions.Length <= 0)
      return false;

    return !MatchesAnyExcludePattern(path);
  }

  private bool MatchesAnyExcludePattern(string path)
  {

    foreach (var expression in _config.SkipPathExpressions)
    {
      if (!Regex.IsMatch(path, expression, RegexOptions.IgnoreCase | RegexOptions.Singleline))
        continue;

      _logger.LogDebug("Skipping path based on RegEx pattern ({rxp}): {path}", expression, path);
      return true;
    }

    return false;
  }

  private static double GetFileSizeKb(IFileInfo fi) =>
    Math.Round(((double)fi.Length / 1024), 4);

  private static double GetFileSizeMb(IFileInfo fi) =>
    Math.Round(((double)fi.Length / 1048576), 4);

  private static double GetFileSizeGb(IFileInfo fi) =>
    Math.Round(((double)fi.Length / 1073741824), 4);
}

using System.Text.RegularExpressions;
using NasFileIndexer.Common.Extensions;
using NasFileIndexer.Common.Mappers;
using NasFileIndexer.Common.Models;
using NasFileIndexer.Common.Repo;
using RnCore.Logging;

namespace NasFileIndexer.Common;

public interface IDirectoryScanner
{
  IDirectoryScanner Configure(NasFileIndexerConfig config);
  Task ScanAsync(string path, CancellationToken stoppingToken);
}

public class DirectoryScanner : IDirectoryScanner
{
  private readonly ILoggerAdapter<DirectoryScanner> _logger;
  private readonly IFileRepo _fileRepo;
  private readonly IRepoObjectMapper _mapper;
  private NasFileIndexerConfig _config = new();

  public DirectoryScanner(ILoggerAdapter<DirectoryScanner> logger,
    IFileRepo fileRepo,
    IRepoObjectMapper mapper)
  {
    _logger = logger;
    _fileRepo = fileRepo;
    _mapper = mapper;
  }

  public IDirectoryScanner Configure(NasFileIndexerConfig config)
  {
    _config = config;
    return this;
  }

  public async Task ScanAsync(string path, CancellationToken stoppingToken)
  {
    if (stoppingToken.IsCancellationRequested)
      return;

    await ScanDirRecursive(path, 1, stoppingToken);
  }


  // Internal scanning methods
  private async Task ScanDirRecursive(string path, int depth, CancellationToken stoppingToken)
  {
    if (!CanScanDirectory(path, depth, stoppingToken))
      return;

    try
    {
      _logger.LogDebug("Scanning directory depth {depth}: {path}", depth, path);
      var directory = new DirectoryInfo(path);

      // Recurse index directories
      var scanTasks = directory
        .GetDirectories()
        .Select(subDirInfo => ScanDirRecursive(subDirInfo.FullName, depth + 1, stoppingToken))
        .ToList();

      if (scanTasks.Count > 1)
        _logger.LogTrace("Waiting on {count} tasks(s) to complete", scanTasks.Count);

      await Task.WhenAll(scanTasks);

      // Index top-level files
      var fileInfos = directory.GetFiles();

      var files = fileInfos
        .Select(info => _mapper.MapFileEntry(info))
        .ToList();

      await SaveResultsAsync(files, stoppingToken);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to index directory '{path}': {error}", path, ex.StackTrace);
    }
  }

  private async Task SaveResultsAsync(List<FileEntity> files, CancellationToken stoppingToken)
  {
    if (stoppingToken.IsCancellationRequested || files.Count == 0)
      return;

    _logger.LogDebug("Saving {count} file(s) to the DB", files.Count);
    await _fileRepo.AddManyAsync(files);
  }

  private bool CanScanDirectory(string path, int depth, CancellationToken stoppingToken)
  {
    // Check to see if a STOP was requested
    if (stoppingToken.IsCancellationRequested)
      return false;

    // Check to see if we have hit the max folder depth
    if (depth > _config.MaxScanDepth)
    {
      _logger.LogInformation("Skipping DIR '{path}' (exceeds max scan depth of {depth})",
        path, _config.MaxScanDepth);

      return false;
    }

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
}

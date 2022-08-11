using System.Text.RegularExpressions;
using NasFileIndexer.Common.Mappers;
using NasFileIndexer.Common.Models;
using NasFileIndexer.Common.Repo;
using Rn.NetCore.Common.Extensions;
using Rn.NetCore.Common.Factories;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Wrappers;

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
  private readonly IIOFactory _ioFactory;
  private NasFileIndexerConfig _config = new();

  public DirectoryScanner(ILoggerAdapter<DirectoryScanner> logger,
    IFileRepo fileRepo,
    IRepoObjectMapper mapper,
    IIOFactory ioFactory)
  {
    _logger = logger;
    _fileRepo = fileRepo;
    _mapper = mapper;
    _ioFactory = ioFactory;
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
      _logger.LogInformation("Scanning directory depth {depth}: {path}", depth, path);
      IDirectoryInfo directory = _ioFactory.GetDirectoryInfo(path);

      // Recurse index directories
      var scanTasks = directory
        .GetDirectories()
        .Select(subDirInfo => ScanDirRecursive(subDirInfo.FullName, depth + 1, stoppingToken))
        .ToList();

      if (scanTasks.Count > 1)
        _logger.LogTrace("Waiting on {count} tasks(s) to complete", scanTasks.Count);

      await Task.WhenAll(scanTasks);

      // Index top-level files
      var files = new List<FileEntity>();
      files.AddRange(directory
        .GetFiles()
        .Select(_mapper.MapFileEntry));

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
}

using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using NasFileIndexer.Common.Mappers;
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
  private readonly IServiceProvider _serviceProvider;
  private readonly IDateTimeAbstraction _dateTime;
  private readonly IRepoObjectMapper _mapper;
  private readonly IIOFactory _ioFactory;
  private readonly IFileRepo _fileRepo;
  private readonly NasFileIndexerConfig _config;
  private DateTime _nextScanTime;

  public FileScannerService(ILoggerAdapter<FileScannerService> logger,
    IDateTimeAbstraction dateTime,
    IIOFactory ioFactory,
    IConfigProvider configProvider,
    IFileRepo fileRepo,
    IServiceProvider serviceProvider,
    IRepoObjectMapper mapper)
  {
    _logger = logger;
    _dateTime = dateTime;
    _ioFactory = ioFactory;
    _fileRepo = fileRepo;
    _serviceProvider = serviceProvider;
    _mapper = mapper;

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

    await _serviceProvider
      .GetRequiredService<IDirectoryScanner>()
      .ScanAsync(_config, path, stoppingToken);

    Console.WriteLine();
    Console.WriteLine();

    try
    {
      _logger.LogInformation("Scanning directory depth {depth}: {path}", depth, path);
      IDirectoryInfo directory = _ioFactory.GetDirectoryInfo(path);

      foreach (IDirectoryInfo subDirInfo in directory.GetDirectories())
        await ScanDirRecursive(subDirInfo.FullName, depth + 1, stoppingToken);

      var files = new List<FileEntity>();
      files.AddRange(directory.GetFiles().Select(_mapper.MapFileEntry));

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

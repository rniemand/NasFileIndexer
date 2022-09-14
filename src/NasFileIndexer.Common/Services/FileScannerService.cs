using Microsoft.Extensions.DependencyInjection;
using NasFileIndexer.Common.Models;
using NasFileIndexer.Common.Repo;
using RnCore.Logging;

namespace NasFileIndexer.Common.Services;

public interface IFileScannerService
{
  Task TickAsync(CancellationToken stoppingToken);
}

public class FileScannerService : IFileScannerService
{
  private readonly ILoggerAdapter<FileScannerService> _logger;
  private readonly IServiceProvider _serviceProvider;
  private readonly IFileRepo _fileRepo;
  private readonly NasFileIndexerConfig _config;
  private DateTime _nextScanTime;

  public FileScannerService(ILoggerAdapter<FileScannerService> logger,
    NasFileIndexerConfig config,
    IFileRepo fileRepo,
    IServiceProvider serviceProvider)
  {
    _logger = logger;
    _fileRepo = fileRepo;
    _serviceProvider = serviceProvider;

    _config = config;
    _nextScanTime = DateTime.Now.AddSeconds(1);

#if DEBUG
    _nextScanTime = DateTime.MinValue;
#endif
  }

  public async Task TickAsync(CancellationToken stoppingToken)
  {
    if (DateTime.Now < _nextScanTime)
      return;

    _logger.LogInformation("Starting to index {count} source(s)",
      _config.ScanPaths.Length);

    // Ensure that we start with a clean "Files" DB table
    await _fileRepo.TruncateTableAsync();

    Parallel.ForEach(_config.ScanPaths, async path =>
      await ScanDirRecursive(path, stoppingToken));

    _logger.LogInformation("Indexing completed");
    _nextScanTime = DateTime.Now.AddHours(24);
    await Task.CompletedTask;
  }


  // Internal methods
  private async Task ScanDirRecursive(string path, CancellationToken stoppingToken)
  {
    await _serviceProvider
      .GetRequiredService<IDirectoryScanner>()
      .Configure(_config)
      .ScanAsync(path, stoppingToken);
  }
}

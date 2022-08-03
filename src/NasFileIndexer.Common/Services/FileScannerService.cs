using NasFileIndexer.Common.Models;
using NasFileIndexer.Common.Providers;
using NasFileIndexer.Common.Repo;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Factories;
using Rn.NetCore.Common.Logging;

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
  private DateTime _nextScanTime = DateTime.MinValue;

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
  }

  public async Task TickAsync(CancellationToken stoppingToken)
  {
    if (_dateTime.Now < _nextScanTime)
      return;

    await _fileRepo.TruncateTableAsync();

    foreach (var scanPath in _config.ScanPaths)
    {
      var files = new List<FileEntity>();
      await ScanDirRecursive(files, scanPath, 1, stoppingToken);
      await EnrichEntriesAsync(files, stoppingToken);
      await SaveResultsAsync(files, stoppingToken);
    }

    _nextScanTime = _dateTime.Now.AddHours(12);
  }

  private async Task ScanDirRecursive(List<FileEntity> files, string path, int depth, CancellationToken stoppingToken)
  {
    if (stoppingToken.IsCancellationRequested || depth > _config.MaxScanDepth)
      return;

    _logger.LogDebug("Scanning directory depth {depth}: {path}", depth, path);
    var directory = _ioFactory.GetDirectoryInfo(path);

    foreach (var subDirInfo in directory.GetDirectories())
      await ScanDirRecursive(files, subDirInfo.FullName, depth+1, stoppingToken);

    files.AddRange(directory
      .GetFiles()
      .Select(fileInfo => new FileEntity
      {
        CreationTimeUtc = fileInfo.CreationTimeUtc,
        DirectoryName = fileInfo.DirectoryName ?? string.Empty,
        Extension = fileInfo.Extension,
        FileName = fileInfo.Name,
        FileSize = fileInfo.Length,
        LastAccessTimeUtc = fileInfo.LastAccessTimeUtc,
        LastWriteTimeUtc = fileInfo.LastWriteTimeUtc
      }));
  }

  private async Task EnrichEntriesAsync(List<FileEntity> files, CancellationToken stoppingToken)
  {
    _logger.LogDebug("Enriching {count} file(s)", files.Count);
    await Task.CompletedTask;
  }

  private async Task SaveResultsAsync(List<FileEntity> files, CancellationToken stoppingToken)
  {
    _logger.LogInformation("Saving {count} file(s) to the DB", files.Count);

    foreach (var file in files)
    {
      if (stoppingToken.IsCancellationRequested)
        return;

      await _fileRepo.AddAsync(file);
    }
  }
}

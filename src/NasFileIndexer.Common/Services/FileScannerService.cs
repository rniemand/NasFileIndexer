using NasFileIndexer.Common.Models;
using NasFileIndexer.Common.Providers;
using Rn.NetCore.Common.Abstractions;
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
  private readonly NasFileIndexerConfig _config;

  public FileScannerService(ILoggerAdapter<FileScannerService> logger,
    IDateTimeAbstraction dateTime,
    IIOFactory ioFactory,
    IConfigProvider configProvider)
  {
    _logger = logger;
    _dateTime = dateTime;
    _ioFactory = ioFactory;
    _config = configProvider.GetConfig();
  }

  public async Task TickAsync(CancellationToken stoppingToken)
  {

    foreach (var scanPath in _config.ScanPaths)
    {
      var files = new List<IFileInfo>();
      await ScanDirRecursive(files, scanPath, stoppingToken);


      var fileInfo = files.First();


      Console.WriteLine();
      Console.WriteLine();
    }






    Console.WriteLine();
    await Task.CompletedTask;
  }

  private async Task ScanDirRecursive(List<IFileInfo> files, string path, CancellationToken stoppingToken)
  {
    if (stoppingToken.IsCancellationRequested)
      return;

    var directory = _ioFactory.GetDirectoryInfo(path);

    foreach (var subDirInfo in directory.GetDirectories())
      await ScanDirRecursive(files, subDirInfo.FullName, stoppingToken);

    files.AddRange(directory.GetFiles());
  }
}

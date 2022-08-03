using NasFileIndexer.Common.Providers;
using NasFileIndexer.Common.Services;
using Rn.NetCore.Common.Logging;

namespace NasFileIndexer
{
  public class Worker : BackgroundService
  {
    private readonly IFileScannerService _fileScannerService;

    public Worker(IFileScannerService fileScannerService)
    {
      _fileScannerService = fileScannerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        await _fileScannerService.TickAsync(stoppingToken);
        await Task.Delay(1000, stoppingToken);
      }
    }
  }
}

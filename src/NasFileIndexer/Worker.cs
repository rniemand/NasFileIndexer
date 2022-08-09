using NasFileIndexer.Common.Services;

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
        await Task.Delay(5000, stoppingToken);
      }
    }
  }
}

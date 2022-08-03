using NasFileIndexer.Common.Providers;
using Rn.NetCore.Common.Logging;

namespace NasFileIndexer
{
  public class Worker : BackgroundService
  {
    private readonly ILoggerAdapter<Worker> _logger;

    public Worker(ILoggerAdapter<Worker> logger, IConfigProvider configProvider)
    {
      _logger = logger;
      var config = configProvider.GetConfig();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        await Task.Delay(1000, stoppingToken);
      }
    }
  }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NasFileIndexer.Common.Providers;
using NasFileIndexer.Common.Services;
using NLog.Extensions.Logging;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Factories;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.DbCommon;

namespace NasFileIndexer.Common.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddNasFileIndexer(this IServiceCollection services, IConfiguration config)
  {
    return services
      // Configuration
      .AddSingleton(config)

      // Abstractions
      .AddSingleton<IFileAbstraction, FileAbstraction>()
      .AddSingleton<IDateTimeAbstraction, DateTimeAbstraction>()
      .AddSingleton<IIOFactory, IOFactory>()

      // DB Stuff
      .AddRnDbMySql(config)

      // Providers
      .AddSingleton<IConfigProvider, ConfigProvider>()

      // Services
      .AddSingleton<IFileScannerService, FileScannerService>()

      // Logging
      .AddLogging(loggingBuilder =>
      {
        loggingBuilder.ClearProviders();
        loggingBuilder.SetMinimumLevel(LogLevel.Trace);
        loggingBuilder.AddNLog(config);
      })

      .AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
  }

  public static IServiceCollection AddNasFileIndexer(this IServiceCollection services) =>
    services.AddNasFileIndexer(BuildDefaultConfiguration());

  private static IConfiguration BuildDefaultConfiguration() =>
    new ConfigurationBuilder()
      .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
      .AddJsonFile("NasFileIndexer.json", true, true)
      .Build();
}

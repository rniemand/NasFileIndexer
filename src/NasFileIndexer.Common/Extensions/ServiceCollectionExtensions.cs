using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NasFileIndexer.Common.Mappers;
using NasFileIndexer.Common.Providers;
using NasFileIndexer.Common.Queries;
using NasFileIndexer.Common.Repo;
using NasFileIndexer.Common.Services;
using NLog.Extensions.Logging;
using RnCore.Logging;

namespace NasFileIndexer.Common.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddNasFileIndexer(this IServiceCollection services, IConfiguration config)
  {
    return services
      // Configuration
      .AddSingleton(config)

      // DB Stuff
      .AddSingleton<IFileRepoQueries, FileRepoQueries>()
      .AddSingleton<IFileRepo, FileRepo>()

      // Providers
      .AddSingleton<IConfigProvider, ConfigProvider>()
      .AddTransient<IDirectoryScanner, DirectoryScanner>()

      // Services
      .AddSingleton<IFileScannerService, FileScannerService>()

      // Mappers
      .AddSingleton<IRepoObjectMapper, RepoObjectMapper>()

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

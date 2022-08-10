using Microsoft.Extensions.Configuration;
using NasFileIndexer.Common.Models;
using Rn.NetCore.Common.Logging;

namespace NasFileIndexer.Common.Providers;

public interface IConfigProvider
{
  NasFileIndexerConfig GetConfig();
}

public class ConfigProvider : IConfigProvider
{
  private readonly ILoggerAdapter<ConfigProvider> _logger;
  private readonly IConfiguration _config;

  public ConfigProvider(ILoggerAdapter<ConfigProvider> logger, IConfiguration config)
  {
    _config = config;
    _logger = logger;
  }

  public NasFileIndexerConfig GetConfig()
  {
    var boundConfig = new NasFileIndexerConfig();
    IConfigurationSection? section = _config.GetSection("NasFileIndexer");

    if (section.Exists())
      section.Bind(boundConfig);

    if (boundConfig.SkipPathExpressions.Length > 0)
      _logger.LogInformation("Loaded {count} exclude patterns: {patterns}",
        boundConfig.SkipPathExpressions.Length,
        string.Join(" | ", boundConfig.SkipPathExpressions));

    return boundConfig;
  }
}

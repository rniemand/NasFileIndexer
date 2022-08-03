using Microsoft.Extensions.Configuration;
using NasFileIndexer.Common.Models;

namespace NasFileIndexer.Common.Providers;

public interface IConfigProvider
{
  NasFileIndexerConfig GetConfig();
}

public class ConfigProvider : IConfigProvider
{
  private readonly IConfiguration _config;

  public ConfigProvider(IConfiguration config)
  {
    _config = config;
  }

  public NasFileIndexerConfig GetConfig()
  {
    var boundConfig = new NasFileIndexerConfig();

    var section = _config.GetSection("NasFileIndexer");
    if (section.Exists())
      section.Bind(boundConfig);

    return boundConfig;
  }
}

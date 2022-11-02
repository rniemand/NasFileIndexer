using Microsoft.Extensions.Configuration;

namespace NasFileIndexer.Common.Models;

public class NasFileIndexerConfig
{
  [ConfigurationKeyName("ScanPaths")]
  public string[] ScanPaths { get; set; } = Array.Empty<string>();

  [ConfigurationKeyName("MaxScanDepth")]
  public int MaxScanDepth { get; set; } = 10;

  [ConfigurationKeyName("SkipPaths")]
  public string[] SkipPaths { get; set; } = Array.Empty<string>();

  [ConfigurationKeyName("SkipPathExpressions")]
  public string[] SkipPathExpressions { get; set; } = Array.Empty<string>();

  [ConfigurationKeyName("AppendMediaInfo")]
  public bool AppendMediaInfo { get; set; } = false;

  [ConfigurationKeyName("UseNullLogger")]
  public bool UseNullLogger { get; set; } = true;
}

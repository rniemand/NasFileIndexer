using Microsoft.Extensions.Configuration;

namespace NasFileIndexer.Common.Models;

public class NasFileIndexerConfig
{
  [ConfigurationKeyName("ScanPaths")]
  public string[] ScanPaths { get; set; } = Array.Empty<string>();

  [ConfigurationKeyName("MaxScanDepth")]
  public int MaxScanDepth { get; set; } = 10;
}

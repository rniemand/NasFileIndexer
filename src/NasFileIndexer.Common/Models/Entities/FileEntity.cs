namespace NasFileIndexer.Common.Models;

public class FileEntity
{
  public long FileId { get; set; }
  public DateTime CreationTimeUtc { get; set; }
  public DateTime LastAccessTimeUtc { get; set; }
  public DateTime LastWriteTimeUtc { get; set; }
  public long FileSize { get; set; }
  public string Extension { get; set; } = string.Empty;
  public string DirectoryName { get; set; } = string.Empty;
  public string FileName { get; set; } = string.Empty;
}

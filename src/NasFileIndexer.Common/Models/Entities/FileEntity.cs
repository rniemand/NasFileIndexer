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
  public string PathSegment01 { get; set; } = string.Empty;
  public string PathSegment02 { get; set; } = string.Empty;
  public string PathSegment03 { get; set; } = string.Empty;
  public string PathSegment04 { get; set; } = string.Empty;
  public string PathSegment05 { get; set; } = string.Empty;
  public string PathSegment06 { get; set; } = string.Empty;
  public string PathSegment07 { get; set; } = string.Empty;
  public string PathSegment08 { get; set; } = string.Empty;
  public string PathSegment09 { get; set; } = string.Empty;
  public string PathSegment10 { get; set; } = string.Empty;
  public string PathSegment11 { get; set; } = string.Empty;
  public string PathSegment12 { get; set; } = string.Empty;
  public string PathSegment13 { get; set; } = string.Empty;
  public string PathSegment14 { get; set; } = string.Empty;
  public string PathSegment15 { get; set; } = string.Empty;
}

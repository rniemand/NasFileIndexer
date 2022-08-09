namespace NasFileIndexer.Common.Models;

public class FileEntity
{
  public long FileId { get; set; }
  public DateTime CreationTimeUtc { get; set; }
  public DateTime LastAccessTimeUtc { get; set; }
  public DateTime LastWriteTimeUtc { get; set; }
  public long FileSize { get; set; }
  public string? Extension { get; set; } = string.Empty;
  public string? FileName { get; set; } = string.Empty;
  public string? PathSegment01 { get; set; }
  public string? PathSegment02 { get; set; }
  public string? PathSegment03 { get; set; }
  public string? PathSegment04 { get; set; }
  public string? PathSegment05 { get; set; }
  public string? PathSegment06 { get; set; }
  public string? PathSegment07 { get; set; }
  public string? PathSegment08 { get; set; }
  public string? PathSegment09 { get; set; }
  public string? PathSegment10 { get; set; }
  public string? PathSegment11 { get; set; }
  public string? PathSegment12 { get; set; }
  public string? PathSegment13 { get; set; }
  public string? PathSegment14 { get; set; }
  public string? PathSegment15 { get; set; }
}

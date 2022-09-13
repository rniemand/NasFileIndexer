namespace NasFileIndexer.Common.Models;

public class FileEntity
{
  public long FileId { get; set; }
  public DateTime CreationTimeUtc { get; set; }
  public DateTime LastAccessTimeUtc { get; set; }
  public DateTime LastWriteTimeUtc { get; set; }
  public long FileSize { get; set; }
  public double FileSizeKb { get; set; }
  public double FileSizeMb { get; set; }
  public double FileSizeGb { get; set; }
  public string? Extension { get; set; } = string.Empty;
  public string? FileName { get; set; } = string.Empty;
  public string FilePath { get; set; } = string.Empty;
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
  public string VideoResolution { get; set; } = string.Empty;
  public bool IsVideoFile { get; set; }
  public bool HasSubtitles { get; set; }
  public double FrameRate { get; set; }
  public int VideoWidth { get; set; }
  public int VideoHeight { get; set; }
  public int AudioStreamCount { get; set; }
  public int VideoStreamCount { get; set; }
  public int SubtitleCount { get; set; }
  public string VideoDuration { get; set; } = string.Empty;
  public double VideoDurationSec { get; set; }
}

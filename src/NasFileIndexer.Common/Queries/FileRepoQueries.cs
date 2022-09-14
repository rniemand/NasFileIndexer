namespace NasFileIndexer.Common.Queries;

public interface IFileRepoQueries
{
  string TruncateTable();
  string Add();
}

public class FileRepoQueries : IFileRepoQueries
{
  public string TruncateTable() => @"TRUNCATE TABLE `Files`;";

  public string Add() => @"INSERT INTO `Files`
    (
	  `CreationTimeUtc`, `LastAccessTimeUtc`, `LastWriteTimeUtc`, `Extension`, `FileName`,
      `PathSegment01`, `PathSegment02`, `PathSegment03`, `PathSegment04`, `PathSegment05`,
      `PathSegment06`, `PathSegment07`, `PathSegment08`, `PathSegment09`, `PathSegment10`,
      `FileSize`, `FileSizeKb`, `FileSizeMb`, `FileSizeGb`, `FilePath`, `VideoResolution`,
      `IsVideoFile`, `HasSubtitles`, `FrameRate`, `VideoWidth`, `VideoHeight`, `AudioStreamCount`,
      `VideoStreamCount`, `SubtitleCount`, `VideoDuration`, `VideoDurationSec`, `VideoFormat`,
      `VideoFormatVersion`
    ) VALUES (
	  @CreationTimeUtc, @LastAccessTimeUtc, @LastWriteTimeUtc, @Extension, @FileName,
      @PathSegment01, @PathSegment02, @PathSegment03, @PathSegment04, @PathSegment05,
      @PathSegment06, @PathSegment07, @PathSegment08, @PathSegment09, @PathSegment10,
      @FileSize, @FileSizeKb, @FileSizeMb, @FileSizeGb, @FilePath, @VideoResolution,
      @IsVideoFile, @HasSubtitles, @FrameRate, @VideoWidth, @VideoHeight, @AudioStreamCount,
      @VideoStreamCount, @SubtitleCount, @VideoDuration, @VideoDurationSec, @VideoFormat,
      @VideoFormatVersion
    );";
}

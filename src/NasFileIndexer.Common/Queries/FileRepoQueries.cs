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
      `PathSegment11`, `PathSegment12`, `PathSegment13`, `PathSegment14`, `PathSegment15`,
      `FileSize`, `FileSizeKb`, `FileSizeMb`, `FileSizeGb`, `FilePath`
    ) VALUES (
	  @CreationTimeUtc, @LastAccessTimeUtc, @LastWriteTimeUtc, @Extension, @FileName,
      @PathSegment01, @PathSegment02, @PathSegment03, @PathSegment04, @PathSegment05,
      @PathSegment06, @PathSegment07, @PathSegment08, @PathSegment09, @PathSegment10,
      @PathSegment11, @PathSegment12, @PathSegment13, @PathSegment14, @PathSegment15,
      @FileSize, @FileSizeKb, @FileSizeMb, @FileSizeGb, @FilePath
    );";
}

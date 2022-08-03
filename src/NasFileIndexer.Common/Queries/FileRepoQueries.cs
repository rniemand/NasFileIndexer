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
	    `CreationTimeUtc`,
	    `LastAccessTimeUtc`,
	    `LastWriteTimeUtc`,
	    `FileSize`,
	    `Extension`,
	    `DirectoryName`,
	    `FileName`
    ) VALUES (
	    @CreationTimeUtc,
	    @LastAccessTimeUtc,
	    @LastWriteTimeUtc,
	    @FileSize,
	    @Extension,
	    @DirectoryName,
	    @FileName
    );";
}

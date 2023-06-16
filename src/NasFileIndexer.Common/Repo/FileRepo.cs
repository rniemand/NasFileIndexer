using Dapper;
using MySqlConnector;
using NasFileIndexer.Common.Models;

namespace NasFileIndexer.Common.Repo;

public interface IFileRepo
{
  Task<int> TruncateTableAsync();
  Task<int> AddAsync(FileEntity fileEntity);
  Task<int> AddManyAsync(List<FileEntity> entries);
}

public class FileRepo : IFileRepo
{
  public const string TableName = "Files";
  private readonly MySqlConnection _connection;

  public FileRepo(IConnectionFactory connectionFactory)
  {
    _connection = connectionFactory.GetConnection();
  }

  public Task<int> TruncateTableAsync()
  {
    const string query = $"TRUNCATE TABLE `{TableName}`;";
    return _connection.ExecuteAsync(query);
  }

  public Task<int> AddAsync(FileEntity fileEntity)
  {
    const string query = $@"INSERT INTO `{TableName}`
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
    return _connection.ExecuteAsync(query, fileEntity);
  }

  public Task<int> AddManyAsync(List<FileEntity> entries)
  {
    const string query = $@"INSERT INTO `{TableName}`
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
    return _connection.ExecuteAsync(query, entries);
  }
}

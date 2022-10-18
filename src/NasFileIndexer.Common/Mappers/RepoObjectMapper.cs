using MediaInfo;
using NasFileIndexer.Common.Models;
using NasFileIndexer.Common.Extensions;
using RnCore.Logging;

namespace NasFileIndexer.Common.Mappers;

public interface IRepoObjectMapper
{
  FileEntity MapFileEntry(FileInfo fileInfo);
}

public class RepoObjectMapper : IRepoObjectMapper
{
  private readonly NasFileIndexerConfig _config;
  private readonly ILoggerAdapter<RepoObjectMapper> _logger;

  public RepoObjectMapper(NasFileIndexerConfig config,
    ILoggerAdapter<RepoObjectMapper> logger)
  {
    _config = config;
    _logger = logger;
  }

  public FileEntity MapFileEntry(FileInfo fileInfo)
  {
    var pathParts = ExtractPathParts(fileInfo.FullName);

    return AppendMediaInfo(new FileEntity
    {
      CreationTimeUtc = fileInfo.CreationTimeUtc,
      Extension = fileInfo.Extension,
      FileName = fileInfo.Name,
      FileSize = fileInfo.Length,
      FileSizeKb = GetFileSizeKb(fileInfo),
      FileSizeMb = GetFileSizeMb(fileInfo),
      FileSizeGb = GetFileSizeGb(fileInfo),
      LastAccessTimeUtc = fileInfo.LastAccessTimeUtc,
      LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
      FilePath = fileInfo.FullName,
      PathSegment01 = pathParts.Length >= 1 ? pathParts[0] : null,
      PathSegment02 = pathParts.Length >= 2 ? pathParts[1] : null,
      PathSegment03 = pathParts.Length >= 3 ? pathParts[2] : null,
      PathSegment04 = pathParts.Length >= 4 ? pathParts[3] : null,
      PathSegment05 = pathParts.Length >= 5 ? pathParts[4] : null,
      PathSegment06 = pathParts.Length >= 6 ? pathParts[5] : null,
      PathSegment07 = pathParts.Length >= 7 ? pathParts[6] : null,
      PathSegment08 = pathParts.Length >= 8 ? pathParts[7] : null,
      PathSegment09 = pathParts.Length >= 9 ? pathParts[8] : null,
      PathSegment10 = pathParts.Length >= 10 ? pathParts[9] : null
    });
  }
  
  private FileEntity AppendMediaInfo(FileEntity file)
  {
    if (!_config.AppendMediaInfo || !IsSupportedMediaFile(file))
      return file;

    try
    {
      var info = new MediaInfoWrapper(file.FilePath, _logger);
      file.VideoStreamCount = info.VideoStreams.Count;

      if (file.VideoStreamCount > 0)
      {
        var mainVideoStream = info.VideoStreams
          .OrderByDescending(x => x.Duration)
          .First();

        file.IsVideoFile = true;
        file.FrameRate = mainVideoStream.FrameRate;
        file.VideoWidth = mainVideoStream.Width;
        file.VideoHeight = mainVideoStream.Height;
        file.VideoDuration = mainVideoStream.Duration.ToString("g");
        file.VideoDurationSec = mainVideoStream.Duration.TotalSeconds;
        file.VideoResolution = mainVideoStream.Resolution;
      }

      file.AudioStreamCount = info.AudioStreams.Count;
      file.SubtitleCount = info.Subtitles.Count;
      file.HasSubtitles = file.SubtitleCount > 0;
      file.VideoFormat = info.Format;
      file.VideoFormatVersion = info.FormatVersion;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to enrich: {path}", file.FilePath);
    }

    return file;
  }

  private static string[] ExtractPathParts(string filePath)
  {
    var baseDir = (Path.GetDirectoryName(filePath) ?? "")
      .Replace("\\", "/")
      .Replace("//", "/");

    var pathParts = new List<string>();

    while (baseDir.Contains("/") || baseDir.Contains("\\"))
    {
      var currentDir = Path.GetFileName(baseDir);

      if (!string.IsNullOrEmpty(currentDir))
        pathParts.Add(currentDir);

      baseDir = Path.GetDirectoryName(baseDir) ?? string.Empty;
    }

    pathParts.Reverse();
    return pathParts.ToArray();
  }

  private static double GetFileSizeKb(FileInfo fi) =>
    Math.Round(((double)fi.Length / 1024), 2);

  private static double GetFileSizeMb(FileInfo fi) =>
    Math.Round(((double)fi.Length / 1048576), 2);

  private static double GetFileSizeGb(FileInfo fi) =>
    Math.Round(((double)fi.Length / 1073741824), 2);

  private static bool IsSupportedMediaFile(FileEntity file) =>
    file.Extension.IgnoreCaseEquals(".mkv");
}

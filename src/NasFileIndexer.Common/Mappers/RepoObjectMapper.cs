using NasFileIndexer.Common.Models;
using Rn.NetCore.Common.Wrappers;

namespace NasFileIndexer.Common.Mappers;

public interface IRepoObjectMapper
{
  FileEntity MapFileEntry(IFileInfo fileInfo);
}

public class RepoObjectMapper : IRepoObjectMapper
{
  public FileEntity MapFileEntry(IFileInfo fileInfo)
  {
    var pathParts = ExtractPathParts(fileInfo.FullName);

    return new FileEntity
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
      PathSegment10 = pathParts.Length >= 10 ? pathParts[9] : null,
      PathSegment11 = pathParts.Length >= 11 ? pathParts[10] : null,
      PathSegment12 = pathParts.Length >= 12 ? pathParts[11] : null,
      PathSegment13 = pathParts.Length >= 13 ? pathParts[12] : null,
      PathSegment14 = pathParts.Length >= 14 ? pathParts[13] : null,
      PathSegment15 = pathParts.Length >= 15 ? pathParts[14] : null
    };
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

  private static double GetFileSizeKb(IFileInfo fi) =>
    Math.Round(((double)fi.Length / 1024), 4);

  private static double GetFileSizeMb(IFileInfo fi) =>
    Math.Round(((double)fi.Length / 1048576), 4);

  private static double GetFileSizeGb(IFileInfo fi) =>
    Math.Round(((double)fi.Length / 1073741824), 4);
}

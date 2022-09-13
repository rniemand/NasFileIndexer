namespace NasFileIndexer.Common.Extensions;

public static class StringExtensions
{
  public static bool IgnoreCaseEquals(this string str, string compare) =>
    str.Equals(compare, StringComparison.InvariantCultureIgnoreCase);
}

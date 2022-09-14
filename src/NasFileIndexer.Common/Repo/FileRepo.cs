using Dapper;
using MySql.Data.MySqlClient;
using NasFileIndexer.Common.Models;
using NasFileIndexer.Common.Queries;

namespace NasFileIndexer.Common.Repo;

public interface IFileRepo
{
  int TruncateTable();
  int Add(FileEntity fileEntity);
  int AddMany(List<FileEntity> entries);
}

public class FileRepo : IFileRepo
{
  private readonly IFileRepoQueries _queries;
  private readonly IConnectionFactory _connectionFactory;

  public FileRepo(IFileRepoQueries queries, IConnectionFactory connectionFactory)
  {
    _queries = queries;
    _connectionFactory = connectionFactory;
  }

  public int TruncateTable()
  {
    using MySqlConnection connection = _connectionFactory.GetConnection();
    return connection.Execute(_queries.TruncateTable());
  }

  public int Add(FileEntity fileEntity)
  {
    using MySqlConnection connection = _connectionFactory.GetConnection();
    return connection.Execute(_queries.Add(), fileEntity);
  }

  public int AddMany(List<FileEntity> entries)
  {
    using MySqlConnection connection = _connectionFactory.GetConnection();
    return connection.Execute(_queries.Add(), entries);
  }
}

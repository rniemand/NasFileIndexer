using Dapper;
using NasFileIndexer.Common.Models;
using NasFileIndexer.Common.Queries;

namespace NasFileIndexer.Common.Repo;

public interface IFileRepo
{
  Task<int> TruncateTableAsync();
  Task<int> AddAsync(FileEntity fileEntity);
  Task<int> AddManyAsync(List<FileEntity> entries);
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

  public Task<int> TruncateTableAsync()
  {
    using var connection = _connectionFactory.GetConnection();
    return connection.ExecuteAsync(_queries.TruncateTable());
  }

  public Task<int> AddAsync(FileEntity fileEntity)
  {
    using var connection = _connectionFactory.GetConnection();
    return connection.ExecuteAsync(_queries.Add(), fileEntity);
  }

  public Task<int> AddManyAsync(List<FileEntity> entries)
  {
    using var connection = _connectionFactory.GetConnection();
    return connection.ExecuteAsync(_queries.Add(), entries);
  }
}

using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
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
  private readonly MySqlConnection _connection;

  public FileRepo(IFileRepoQueries queries, IConfiguration configuration)
  {
    var connectionString = configuration.GetSection("ConnectionStrings:NasFileIndexer").Value ?? string.Empty;
    _connection = new MySqlConnection(connectionString);
    _queries = queries;
  }

  public Task<int> TruncateTableAsync()
  {
    EnsureConnected();
    return _connection.ExecuteAsync(_queries.TruncateTable());
  }

  public Task<int> AddAsync(FileEntity fileEntity)
  {
    EnsureConnected();

    return _connection.ExecuteAsync(_queries.Add(), fileEntity);
  }

  public Task<int> AddManyAsync(List<FileEntity> entries)
  {
    EnsureConnected();
    return _connection.ExecuteAsync(_queries.Add(), entries);
  }

  private void EnsureConnected()
  {
    switch (_connection.State)
    {
      case ConnectionState.Open:
        return;
      case ConnectionState.Closed:
        _connection.Open();
        break;
    }

    if (_connection.State != ConnectionState.Broken)
      return;

    try
    {
      _connection.Close();
    }
    catch (Exception)
    {
      // swallow
    }
    
    _connection.Open();
  }
}

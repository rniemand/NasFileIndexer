using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
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
  private readonly MySqlConnection _connection;
  private readonly object _lockObject = new();

  public FileRepo(IFileRepoQueries queries, IConfiguration configuration)
  {
    var connectionString = configuration.GetSection("ConnectionStrings:NasFileIndexer").Value ?? string.Empty;
    _connection = new MySqlConnection(connectionString);
    _queries = queries;
  }

  public int TruncateTable()
  {
    EnsureConnected();

    int result;
    lock (_lockObject)
      result = _connection.Execute(_queries.TruncateTable());

    return result;
  }

  public int Add(FileEntity fileEntity)
  {
    EnsureConnected();

    var result = 0;
    lock (_lockObject)
      result = _connection.Execute(_queries.Add(), fileEntity);

    return result;
  }

  public int AddMany(List<FileEntity> entries)
  {
    EnsureConnected();

    var result = 0;
    lock (_lockObject)
      result = _connection.Execute(_queries.Add(), entries);

    return result;
  }

  private void EnsureConnected()
  {
    lock (_lockObject)
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
}

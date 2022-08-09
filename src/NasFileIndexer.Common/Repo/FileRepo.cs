using NasFileIndexer.Common.Models;
using NasFileIndexer.Common.Queries;
using Rn.NetCore.DbCommon;

namespace NasFileIndexer.Common.Repo;

public interface IFileRepo
{
  Task<int> TruncateTableAsync();
  Task<int> AddAsync(FileEntity fileEntity);
  Task<int> AddManyAsync(List<FileEntity> entries);
}

public class FileRepo : BaseRepo<FileRepo>, IFileRepo
{
  private readonly IFileRepoQueries _queries;

  public FileRepo(IBaseRepoHelper baseRepoHelper, IFileRepoQueries queries)
    : base(baseRepoHelper)
  {
    _queries = queries;
  }

  public Task<int> TruncateTableAsync() =>
    ExecuteAsync(nameof(TruncateTableAsync), _queries.TruncateTable());

  public Task<int> AddAsync(FileEntity fileEntity) =>
    ExecuteAsync(nameof(AddAsync), _queries.Add(), fileEntity);

  public Task<int> AddManyAsync(List<FileEntity> entries) =>
    ExecuteAsync(nameof(AddManyAsync), _queries.Add(), entries);
}

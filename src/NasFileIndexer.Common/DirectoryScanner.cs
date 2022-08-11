using NasFileIndexer.Common.Mappers;
using NasFileIndexer.Common.Models;
using NasFileIndexer.Common.Repo;
using Rn.NetCore.Common.Logging;

namespace NasFileIndexer.Common;

public interface IDirectoryScanner
{
  Task ScanAsync(NasFileIndexerConfig config, string path, CancellationToken stoppingToken);
}

public class DirectoryScanner : IDirectoryScanner
{
  private readonly ILoggerAdapter<DirectoryScanner> _logger;
  private readonly IFileRepo _fileRepo;
  private readonly IRepoObjectMapper _mapper;

  public DirectoryScanner(ILoggerAdapter<DirectoryScanner> logger,
    IFileRepo fileRepo,
    IRepoObjectMapper mapper)
  {
    _logger = logger;
    _fileRepo = fileRepo;
    _mapper = mapper;
  }

  public async Task ScanAsync(NasFileIndexerConfig config, string path, CancellationToken stoppingToken)
  {



    await Task.CompletedTask;
  }
}

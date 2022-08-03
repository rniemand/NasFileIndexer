using NasFileIndexer;
using NasFileIndexer.Common.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
      services
        .AddNasFileIndexer()
        .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

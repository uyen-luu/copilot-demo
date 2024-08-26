using Api.Azure.OpenAi.Extensions;
using Api.Azure.Search.Extensions;
using Generator;
using Generator.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    // ReSharper disable once AccessToDisposedClosure
    cancellationTokenSource.Cancel();
};
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((host, config) =>
    {
        config.AddJsonFile($"local.settings.json", optional: true);
        config.AddJsonFile($"local.settings.{host.HostingEnvironment.EnvironmentName}.json", optional: true);
    })
    .ConfigureServices(
    services =>
    {
        services.AddOpenAi().AddAiSearch();
        services.AddSingleton<IEntityDataSource<Book>, BookDataSource>();
        services.AddHostedService<VectorDb>();
    });
var host = builder.Build();
await host.RunAsync(cancellationTokenSource.Token);

using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartMetering.Application;
using SmartMetering.Infrastructure;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddApplication();
        services.AddInfrastructure(configuration);

        services.AddSingleton(_ =>
        {
            var connectionString = configuration.GetConnectionString("AzureStorage")!;
            var client = new QueueClient(connectionString, "telemetry-queue",
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
            client.CreateIfNotExists();
            return client;
        });
    })
    .Build();

host.Run();

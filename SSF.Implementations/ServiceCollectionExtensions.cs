using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Serilog.Formatting.Elasticsearch;
using Serilog;
using SSF.Database;
using SSF.Implementations.Database;
using Serilog.Enrichers.Span;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSSFImplementations(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDefaultAWSOptions(configuration.GetAWSOptions())
            .AddAWSService<IAmazonDynamoDB>()
            .AddSingleton<IApiKeyRepository, ApiKeyRepository>()
            .AddSingleton<IWebSocketConnectionRepository, WebSocketConnectionRepository>()
            .AddLogging(builder =>
            {
                var serilogLogger = new LoggerConfiguration()
                    .Enrich.WithSpan()
                    .WriteTo.Console(new ElasticsearchJsonFormatter())
                    .CreateLogger();
                builder.AddSerilog(serilogLogger);
            });

        return services;
    }
}
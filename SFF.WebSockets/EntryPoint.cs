using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFF.WebSockets;

public static class EntryPoint
{
    private static WebSocketLambdaHandler Build()
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection()
            .AddSSFImplementations(configuration)
            .AddSingleton<WebSocketLambdaHandler>()
            .BuildServiceProvider();

        return services.GetRequiredService<WebSocketLambdaHandler>();
    }

    private static WebSocketLambdaHandler _Handler = Build();

    public static async Task<APIGatewayProxyResponse> HandleAsync(APIGatewayProxyRequest request, ILambdaContext lambdaContext) =>
        await _Handler.HandleAsync(request, lambdaContext);
}

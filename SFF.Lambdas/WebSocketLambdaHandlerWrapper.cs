using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using System.Text.Json.Serialization;

[assembly: LambdaSerializer(typeof(SourceGeneratorLambdaJsonSerializer<SFF.Lambdas.WebSocketLambdaHandlerSerializerContext>))]

namespace SFF.Lambdas;

public static class WebSocketLambdaHandlerWrapper
{
    private static WebSocketLambdaHandler _Handler = new WebSocketLambdaHandler();
    public static async Task<APIGatewayProxyResponse> HandleAsync(APIGatewayProxyRequest request, ILambdaContext lambdaContext) =>
        await _Handler.HandleAsync(request, lambdaContext);
}

[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
public partial class WebSocketLambdaHandlerSerializerContext : JsonSerializerContext
{
}
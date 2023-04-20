using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using System.Text.Json.Serialization;

[assembly: LambdaSerializer(typeof(SourceGeneratorLambdaJsonSerializer<SFF.WebSockets.LambdaSerializerContext>))]

namespace SFF.WebSockets;

[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
public partial class LambdaSerializerContext : JsonSerializerContext
{
}
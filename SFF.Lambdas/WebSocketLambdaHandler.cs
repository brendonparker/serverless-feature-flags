using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace SFF.Lambdas;

public delegate Task<APIGatewayProxyResponse> ApiGatewayProxyRoute(APIGatewayProxyRequest request);

public class WebSocketLambdaHandler
{
    private readonly Dictionary<string, ApiGatewayProxyRoute> _routes;
    private readonly Amazon.DynamoDBv2.AmazonDynamoDBClient _client;
    private readonly Table _table;

    public WebSocketLambdaHandler()
    {
        _client = new Amazon.DynamoDBv2.AmazonDynamoDBClient();
        _table = Table.LoadTable(_client, new TableConfig(Constants.TABLE_NAME));

        _routes = new()
        {
            ["$connect"] = HandleConnectAsync,
            ["$disconnect"] = HandleDisconnectAsync,
            ["$default"] = HandleDefaultAsync,
        };
    }

    public Task<APIGatewayProxyResponse> HandleAsync(APIGatewayProxyRequest request, ILambdaContext lambdaContext) =>
        _routes[request.RequestContext.RouteKey](request);

    public async Task<APIGatewayProxyResponse> HandleConnectAsync(APIGatewayProxyRequest request)
    {
        var doc = new Document();
        doc[Constants.PartitionKey] = $"Connection|{request.RequestContext.ConnectionId}";
        doc[Constants.SortKey] = request.RequestContext.ConnectionId;
        doc[Constants.Expiry] = (int)(DateTime.UtcNow.AddHours(4) - DateTime.UnixEpoch).TotalSeconds;

        await _table.PutItemAsync(doc);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200
        };
    }

    public async Task<APIGatewayProxyResponse> HandleDisconnectAsync(APIGatewayProxyRequest request)
    {
        var doc = new Document();
        doc[Constants.PartitionKey] = $"Connection|{request.RequestContext.ConnectionId}";
        doc[Constants.SortKey] = request.RequestContext.ConnectionId;
        await _table.DeleteItemAsync(doc);
        return new APIGatewayProxyResponse
        {
            StatusCode = 200
        };
    }

    public async Task<APIGatewayProxyResponse> HandleDefaultAsync(APIGatewayProxyRequest request)
    {
        await Task.CompletedTask;
        return new APIGatewayProxyResponse
        {
            StatusCode = 200
        };
    }
}

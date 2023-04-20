using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using SSF.Database;

namespace SFF.WebSockets;

public delegate Task<APIGatewayProxyResponse> ApiGatewayProxyRoute(APIGatewayProxyRequest request);

public class WebSocketLambdaHandler
{
    private readonly Dictionary<string, ApiGatewayProxyRoute> _routes;
    private readonly IWebSocketConnectionRepository _webSocketConnectionRepository;
    private readonly IApiKeyRepository _apiKeyRepository;

    public WebSocketLambdaHandler(
        IWebSocketConnectionRepository webSocketConnectionRepository,
        IApiKeyRepository apiKeyRepository)
    {
        _routes = new()
        {
            ["$connect"] = HandleConnectAsync,
            ["$disconnect"] = HandleDisconnectAsync,
            ["$default"] = HandleDefaultAsync,
        };
        _webSocketConnectionRepository = webSocketConnectionRepository;
        _apiKeyRepository = apiKeyRepository;
    }

    public Task<APIGatewayProxyResponse> HandleAsync(APIGatewayProxyRequest request, ILambdaContext lambdaContext) =>
        _routes[request.RequestContext.RouteKey](request);

    public async Task<APIGatewayProxyResponse> HandleConnectAsync(APIGatewayProxyRequest request)
    {
        if (!request.QueryStringParameters.ContainsKey("apiKey")) return new APIGatewayProxyResponse { StatusCode = 401 };

        var apiKey = await _apiKeyRepository.ValidateAndGetApiKeyAsync(request.QueryStringParameters["apiKey"]);

        if (apiKey == null) return new APIGatewayProxyResponse { StatusCode = 401 };

        await _webSocketConnectionRepository.InsertAsync(new SSF.Models.WebSocketConnectionInfo
        {
            ConnectionId = request.RequestContext.ConnectionId,
            Expiry = DateTime.UtcNow.AddHours(4),
            CustomerId = apiKey.CustomerId
        });

        return new APIGatewayProxyResponse
        {
            StatusCode = 200
        };
    }

    public async Task<APIGatewayProxyResponse> HandleDisconnectAsync(APIGatewayProxyRequest request)
    {
        await _webSocketConnectionRepository.DeleteAsync(request.RequestContext.ConnectionId);
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

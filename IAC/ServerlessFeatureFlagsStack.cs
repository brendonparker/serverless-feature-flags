using Amazon.CDK;
using Constructs;
using SFF.InfrastructureAsCode.Constructs;

namespace SFF.Infrastructure;

public class ServerlessFeatureFlagsStack : Stack
{
    internal ServerlessFeatureFlagsStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var apiKey = new ApiKeyManagementConstruct(this, "ApiKey");
        var ws = new WebSocketConstruct(this, "WebSocket", new WebSocketConstructProps
        {
            ApiKeyTable = apiKey.Table
        });
    }
}

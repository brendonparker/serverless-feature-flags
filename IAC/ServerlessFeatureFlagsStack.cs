using Amazon.CDK;
using Constructs;
using SFF.InfrastructureAsCode.Constructs;

namespace SFF.Infrastructure;

public class ServerlessFeatureFlagsStack : Stack
{
    internal ServerlessFeatureFlagsStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var featureFlagManagement = new FeatureFlagManagementConstruct(this, "FeatureFlag");

        var ws = new WebSocketConstruct(this, "WebSocket", new WebSocketConstructProps
        {
            MainTable = featureFlagManagement.Table
        });
    }
}

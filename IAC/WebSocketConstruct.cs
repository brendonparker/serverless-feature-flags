using Amazon.CDK;
using Amazon.CDK.AWS.Apigatewayv2.Alpha;
using Amazon.CDK.AWS.Apigatewayv2.Integrations.Alpha;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using System.Collections.Generic;

namespace SFF.Infrastructure;

public class WebSocketConstructProps
{
    public string TableName { get; set; }
}

public class WebSocketConstruct : Construct
{
    public IFunction LambdaProxy { get; }

    public WebSocketConstruct(Construct scope, string id, WebSocketConstructProps props) : base(scope, id)
    {
        LambdaProxy = new Function(this, "Lambda", new FunctionProps
        {
            Code = Code.FromAsset("LambdaSource/SFF.Lambdas"),
            Handler = "SFF.Lambdas::SFF.Lambdas.WebSocketLambdaHandlerWrapper::HandleAsync",
            Runtime = Runtime.DOTNET_6,
            MemorySize = 1536,
            Environment = new Dictionary<string, string>
            {
                ["TABLE_NAME"] = props.TableName
            }
        });

        var api = new WebSocketApi(this, "WebSocketApi", new WebSocketApiProps
        {
            ConnectRouteOptions = new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration("ConnectIntegration", LambdaProxy)
            },
            DisconnectRouteOptions = new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration("DisconnectIntegration", LambdaProxy)
            },
            DefaultRouteOptions = new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration("DefaultIntegration", LambdaProxy)
            },
        });

        new WebSocketStage(this, "ProdStage", new WebSocketStageProps
        {
            AutoDeploy = true,
            WebSocketApi = api,
            StageName = "prod"
        });

        api.GrantManageConnections(LambdaProxy);

        new CfnOutput(this, "WsApiEndpoint", new CfnOutputProps
        {
            ExportName = "WsApiEndpoint",
            Value = api.ApiEndpoint
        });
    }
}

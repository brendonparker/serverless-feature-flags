using Amazon.CDK;
using Amazon.CDK.AWS.Apigatewayv2.Alpha;
using Amazon.CDK.AWS.Apigatewayv2.Integrations.Alpha;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using System.Collections.Generic;

namespace SFF.InfrastructureAsCode.Constructs;

public class WebSocketConstructProps
{
    public ITable MainTable { get; init; }
}

public class WebSocketConstruct : Construct
{
    public IFunction LambdaProxy { get; }

    public WebSocketConstruct(Construct scope, string id, WebSocketConstructProps props) : base(scope, id)
    {
        var table = new Table(this, "Table", new TableProps
        {
            PartitionKey = new Attribute
            {
                Name = Constants.ConnectionId,
                Type = AttributeType.STRING
            },
            TimeToLiveAttribute = Constants.Expiry,
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        LambdaProxy = new Function(this, "Lambda", new FunctionProps
        {
            Code = Code.FromAsset("LambdaSource/SFF.WebSockets"),
            Handler = "SFF.WebSockets::SFF.WebSockets.EntryPoint::HandleAsync",
            Runtime = Runtime.DOTNET_6,
            MemorySize = 1536,
            Environment = new Dictionary<string, string>
            {
                ["LAMBDA_NET_SERIALIZER_DEBUG"] = "true",
                [Constants.SSF_TABLE_NAME_CONNECTIONS] = table.TableName,
                [Constants.SSF_TABLE_NAME] = props.MainTable.TableName,
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
        table.GrantReadWriteData(LambdaProxy);
        props.MainTable.GrantReadData(LambdaProxy);

        new CfnOutput(this, "WsApiEndpoint", new CfnOutputProps
        {
            ExportName = "WsApiEndpoint",
            Value = api.ApiEndpoint
        });
    }
}

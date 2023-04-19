using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;

namespace SFF.Infrastructure;

public class ServerlessFeatureFlagsStack : Stack
{
    internal ServerlessFeatureFlagsStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var dynamoTable = new Table(this, "DynamoDbTable", new TableProps
        {
            PartitionKey = new Attribute
            {
                Name = "PartitionKey",
                Type = AttributeType.STRING
            },
            SortKey = new Attribute
            {
                Name = "SortKey",
                Type = AttributeType.STRING
            },
            TimeToLiveAttribute = "Expiry",
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        var ws = new WebSocketConstruct(this, "WebSocket", new WebSocketConstructProps
        {
            TableName = dynamoTable.TableName,
        });

        dynamoTable.GrantReadWriteData(ws.LambdaProxy);
    }
}

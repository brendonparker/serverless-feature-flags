using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using System.Collections.Generic;

namespace SFF.InfrastructureAsCode.Constructs;

public class FeatureFlagManagementConstructProps
{
    public ITable ApiKeyTable { get; init; }
}

public class FeatureFlagManagementConstruct : Construct
{
    public IFunction LambdaProxy { get; }

    public FeatureFlagManagementConstruct(Construct scope, string id, FeatureFlagManagementConstructProps props) : base(scope, id)
    {
        var table = new Table(this, "Table", new TableProps
        {
            PartitionKey = new Attribute
            {
                Name = Constants.CustomerId,
                Type = AttributeType.STRING
            },
            SortKey = new Attribute
            {
                Name = "FeatureFlagKey"
            },
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        LambdaProxy = new Function(this, "Lambda", new FunctionProps
        {
            Code = Code.FromAsset("LambdaSource/SFF.FeatureFlagManagement"),
            Handler = "SFF.FeatureFlagManagement",
            Runtime = Runtime.DOTNET_6,
            MemorySize = 1536,
            Environment = new Dictionary<string, string>
            {
                //["LAMBDA_NET_SERIALIZER_DEBUG"] = "true",
                [Constants.TABLE_NAME_API_KEYS] = props.ApiKeyTable.TableName,
            }
        });

        var functionUrl = LambdaProxy.AddFunctionUrl(new FunctionUrlOptions
        {
            AuthType = FunctionUrlAuthType.NONE,
        });

        table.GrantReadWriteData(LambdaProxy);

        new CfnOutput(this, "MgmtApiEndpoint", new CfnOutputProps
        {
            ExportName = "MgmtApiEndpoint",
            Value = functionUrl.Url
        });
    }
}

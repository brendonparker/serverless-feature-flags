using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using System.Collections.Generic;

namespace SFF.InfrastructureAsCode.Constructs;

public class FeatureFlagManagementConstructProps
{
}

public class FeatureFlagManagementConstruct : Construct
{
    public IFunction LambdaProxy { get; }
    public ITable Table { get; }

    public FeatureFlagManagementConstruct(Construct scope, string id, FeatureFlagManagementConstructProps props = null) : base(scope, id)
    {
        props ??= new();

        var table = new Table(this, "Table", new TableProps
        {
            PartitionKey = new Attribute
            {
                Name = Constants.PK,
                Type = AttributeType.STRING
            },
            SortKey = new Attribute
            {
                Name = Constants.SK,
                Type = AttributeType.STRING
            },
            RemovalPolicy = RemovalPolicy.DESTROY,
        });


        table.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
        {
            IndexName = "GSI_ApiKeyHash",
            PartitionKey = new Attribute
            {
                Name = "ApiKeyHash",
                Type = AttributeType.STRING
            }
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
                [Constants.SSF_TABLE_NAME] = table.TableName,
                ["Auth0__Domain"] = "",
                ["Auth0__ClientId"] = "",
            }
        });

        var ssmPolicy = new Policy(this, "SSMPolicy", new PolicyProps
        {
            Document = new PolicyDocument(new PolicyDocumentProps
            {
                Statements = new[]
                {
                    new PolicyStatement(new PolicyStatementProps
                    {
                        Actions = new []
                        {
                            "ssm:PutParameter",
                            "ssm:GetParametersByPath",
                            "ssm:GetParameters",
                            "ssm:GetParameter"
                        },
                        Resources = new []
                        {
                            $"arn:aws:ssm:*:{Aws.ACCOUNT_ID}:parameter/*"
                        },
                    })
                }
            })
        });

        LambdaProxy.Role.AttachInlinePolicy(ssmPolicy);

        var functionUrl = LambdaProxy.AddFunctionUrl(new FunctionUrlOptions
        {
            AuthType = FunctionUrlAuthType.NONE,
        });

        table.GrantReadWriteData(LambdaProxy);

        Table = table;

        new CfnOutput(this, "MgmtApiEndpoint", new CfnOutputProps
        {
            ExportName = "MgmtApiEndpoint",
            Value = functionUrl.Url
        });
    }
}

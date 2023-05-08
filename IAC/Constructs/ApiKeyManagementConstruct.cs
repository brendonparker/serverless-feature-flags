using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;
using System.Collections.Generic;

namespace SFF.InfrastructureAsCode.Constructs;

public class ApiKeyManagementConstructProps
{
}

public class ApiKeyManagementConstruct : Construct
{
    public ITable Table { get; }

    public ApiKeyManagementConstruct(Construct scope, string id, ApiKeyManagementConstructProps props = null) : base(scope, id)
    {
        props ??= new();

        Table = new Table(this, "Table", new TableProps
        {
            PartitionKey = new Attribute
            {
                Name = Constants.PK,
                Type = AttributeType.STRING
            },
            RemovalPolicy = RemovalPolicy.DESTROY,
        });
    }
}

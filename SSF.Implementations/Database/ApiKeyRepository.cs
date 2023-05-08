using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using SFF;
using SFF.Auth;
using SSF.Database;

namespace SSF.Implementations.Database;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly Table _table;
    private readonly IAmazonDynamoDB _client;

    public ApiKeyRepository(IAmazonDynamoDB client)
    {
        _table = Table.LoadTable(client, new TableConfig(Constants.TableName));
        _client = client;
    }

    public async Task<ApiKey> GetApiKeyAsync(string hashKey)
    {
        var search = _table.Query(new QueryOperationConfig
        {
            IndexName = "GSI_ApiKeyHash",
            KeyExpression = new Expression
            {
                ExpressionStatement = "#api_key = :api_key",
                ExpressionAttributeNames = new()
                {
                    ["#api_key"] = "ApiKeyHash"
                },
                ExpressionAttributeValues = new()
                {
                    [":api_key"] = hashKey
                }
            }
        });
        var res = await search.GetNextSetAsync();
        var doc = res.FirstOrDefault();

        if (doc == null)
            return null;

        return new ApiKey
        {
            ApiKeyHash = doc["ApiKeyHash"],
            CustomerId = Util.ParseCustomerId(doc[Constants.PK]),
            EnvironmentId = doc[Constants.EnvironmentId]
        };
    }

    public async Task InsertApiKeyAsync(ApiKey apiKey)
    {
        var doc = new Document(new()
        {
            [Constants.PK] = Util.DeriveCustomerId(apiKey.CustomerId),
            [Constants.SK] = $"e#{apiKey.EnvironmentId}|k#{apiKey.ApiKeyHash}",
            ["ApiKeyHash"] = apiKey.ApiKeyHash,
            [Constants.Type] = "ApiKey",
            [Constants.EnvironmentId] = apiKey.EnvironmentId,
        });
        await _table.PutItemAsync(doc);
    }
}

public static class Util
{
    public static string ParseCustomerId(string input)
    {
        if (input?.StartsWith("c#") == true) return input[2..];
        return input;
    }

    public static string DeriveCustomerId(string input)
    {
        if (input?.StartsWith("c#") == true) return input;
        return $"c#{input}";
    }
}

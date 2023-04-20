using Amazon.DynamoDBv2.DocumentModel;

namespace SFF.Lambdas.Auth;
using static AuthUtils;

public interface IApiKeyGenerator
{
    Task<string> GenerateAsync(string customerId);
}

public class ApiKeyGenerator : IApiKeyGenerator
{
    private readonly Amazon.DynamoDBv2.AmazonDynamoDBClient _client;
    private readonly Table _table;

    public ApiKeyGenerator()
    {
        _client = new Amazon.DynamoDBv2.AmazonDynamoDBClient();
        _table = Table.LoadTable(_client, new TableConfig(Constants.TableApiKeys));
    }

    public async Task<string> GenerateAsync(string customerId)
    {
        var key = Generate();

        var doc = new Document(new()
        {
            [Constants.ApiKeyHash] = ComputeHash(key),
            [Constants.CustomerId] = customerId,
        });

        await _table.PutItemAsync(doc);

        return key;
    }
}

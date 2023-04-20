using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using SFF;
using SFF.Auth;
using SSF.Database;

namespace SSF.Implementations.Database;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly Table _table;

    public ApiKeyRepository(IAmazonDynamoDB client)
    {
        _table = Table.LoadTable(client, new TableConfig(Constants.TableApiKeys));
    }

    public async Task<ApiKey> GetApiKeyAsync(string hashKey)
    {
        var doc = await _table.GetItemAsync(hashKey);

        if (doc == null)
            return null;

        return new ApiKey
        {
            ApiKeyHash = doc[Constants.ApiKeyHash].AsString(),
            CustomerId = doc[Constants.CustomerId].AsString()
        };
    }

    public async Task InsertApiKeyAsync(ApiKey apiKey)
    {
        var doc = new Document(new()
        {
            [Constants.ApiKeyHash] = apiKey.ApiKeyHash,
            [Constants.CustomerId] = apiKey.CustomerId,
        });
        await _table.PutItemAsync(doc);
    }
}

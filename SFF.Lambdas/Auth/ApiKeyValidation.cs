using Amazon.DynamoDBv2.DocumentModel;

namespace SFF.Lambdas.Auth;

using static AuthUtils;

public interface IApiKeyValidation
{
    Task<ApiKey> ValidateAsync(string apiKey);
}

public class ApiKeyValidation : IApiKeyValidation
{
    private readonly Amazon.DynamoDBv2.AmazonDynamoDBClient _client;
    private readonly Table _table;

    public ApiKeyValidation()
    {
        _client = new Amazon.DynamoDBv2.AmazonDynamoDBClient();
        _table = Table.LoadTable(_client, new TableConfig(Constants.TableApiKeys));
    }

    public async Task<ApiKey> ValidateAsync(string apiKey)
    {
        var index = apiKey.LastIndexOf('_');
        if (index == -1) return null;

        var firstPart = apiKey.Substring(0, index);
        var secondPart = apiKey.Substring(index + 1);

        if (secondPart != ComputeCheckSum(firstPart))
            return null;

        var doc = await _table.GetItemAsync(ComputeHash(apiKey));

        if (doc == null)
            return null;

        return new ApiKey
        {
            ApiKeyHash = doc[Constants.ApiKeyHash].AsString(),
            CustomerId = doc[Constants.CustomerId].AsString()
        };
    }
}

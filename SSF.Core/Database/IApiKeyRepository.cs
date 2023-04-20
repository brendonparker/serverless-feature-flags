using SFF.Auth;

namespace SSF.Database;

public interface IApiKeyRepository
{
    Task<ApiKey> GetApiKeyAsync(string apiKeyHash);
    Task InsertApiKeyAsync(ApiKey apiKey);
}

public static class IApiKeyRepositoryExtensions
{
    public static async Task<ApiKey?> ValidateAndGetApiKeyAsync(this IApiKeyRepository db, string apiKey)
    {
        var index = apiKey.LastIndexOf('_');
        if (index == -1) return null;

        var firstPart = apiKey.Substring(0, index);
        var secondPart = apiKey.Substring(index + 1);

        if (secondPart != AuthUtils.ComputeCheckSum(firstPart))
            return null;

        return await db.GetApiKeyAsync(AuthUtils.ComputeHash(apiKey));
    }
}
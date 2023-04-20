﻿using SFF.Auth;
using SSF.Database;

namespace SFF.Lambdas.Auth;

public interface IApiKeyGenerator
{
    Task<string> GenerateAsync(string customerId);
}

public class ApiKeyGenerator : IApiKeyGenerator
{
    private readonly IApiKeyRepository _db;

    public ApiKeyGenerator(IApiKeyRepository db)
    {
        _db = db;
    }

    public async Task<string> GenerateAsync(string customerId)
    {
        var key = AuthUtils.Generate();

        await _db.InsertApiKeyAsync(new ApiKey
        {
            ApiKeyHash = AuthUtils.ComputeHash(key),
            CustomerId = customerId
        });

        return key;
    }
}

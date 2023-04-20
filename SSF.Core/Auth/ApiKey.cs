namespace SFF.Auth;

public sealed class ApiKey
{
    public string ApiKeyHash { get; init; } = string.Empty;
    public string CustomerId { get; init; } = string.Empty;
}
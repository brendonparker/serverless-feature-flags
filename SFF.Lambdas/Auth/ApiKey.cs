namespace SFF.Lambdas.Auth;

public sealed class ApiKey
{
    public string ApiKeyHash { get; init; }
    public string CustomerId { get; init; }
}
namespace SFF;

public sealed class Constants
{
    public const string TABLE_NAME_CONNECTIONS = "TABLE_NAME_CONNECTIONS";
    public const string TABLE_NAME_API_KEYS = "TABLE_NAME_API_KEYS";

    public static readonly string TableConnections = Environment.GetEnvironmentVariable(TABLE_NAME_CONNECTIONS);
    public static readonly string TableApiKeys = Environment.GetEnvironmentVariable(TABLE_NAME_API_KEYS);

    public static readonly string CustomerId = "CustomerId";
    public static readonly string ConnectionId = "ConnectionId";
    public static readonly string ApiKeyHash = "ApiKeyHash";
    public static readonly string Expiry = "Expiry";
}

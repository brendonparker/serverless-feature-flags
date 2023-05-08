namespace SFF;

public sealed class Constants
{
    public const string SSF_TABLE_NAME_CONNECTIONS = "SSF_TABLE_NAME_CONNECTIONS";
    public const string SSF_TABLE_NAME = "SSF_TABLE_NAME";

    public static readonly string TableNameConnections = Environment.GetEnvironmentVariable(SSF_TABLE_NAME_CONNECTIONS);
    public static readonly string TableName = Environment.GetEnvironmentVariable(SSF_TABLE_NAME);

    public static readonly string CustomerId = "CustomerId";
    public static readonly string ConnectionId = "ConnectionId";
    public static readonly string PK = "PK";
    public static readonly string SK = "SK";
    public static readonly string Expiry = "Expiry";
    public static readonly string EnvironmentId = "EnvironmentId";
    public static readonly string Type = "Type";
}

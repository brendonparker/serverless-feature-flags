namespace SFF.Lambdas;

public sealed class Constants
{
    public static readonly string TABLE_NAME = Environment.GetEnvironmentVariable("TABLE_NAME");
    public static readonly string PartitionKey = "PartitionKey";
    public static readonly string SortKey = "SortKey";
    public static readonly string Expiry = "Expiry";
}

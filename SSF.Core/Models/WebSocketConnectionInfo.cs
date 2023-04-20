namespace SSF.Models;

public sealed class WebSocketConnectionInfo
{
    public string ConnectionId { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}

using SSF.Models;

namespace SSF.Database;

public interface IWebSocketConnectionRepository
{
    Task InsertAsync(WebSocketConnectionInfo info);
    Task DeleteAsync(string connectionId);
    Task<WebSocketConnectionInfo> GetAsync(string connectionId);
}

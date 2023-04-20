using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2;
using SSF.Database;
using SFF;
using SSF.Models;

namespace SSF.Implementations.Database;

public class WebSocketConnectionRepository : IWebSocketConnectionRepository
{
    private readonly Table _table;

    public WebSocketConnectionRepository(IAmazonDynamoDB client)
    {
        _table = Table.LoadTable(client, new TableConfig(Constants.TableConnections));
    }

    public async Task InsertAsync(WebSocketConnectionInfo info)
    {
        var doc = new Document(new()
        {
            [Constants.ConnectionId] = info.ConnectionId,
            [Constants.CustomerId] = info.CustomerId,
            [Constants.Expiry] = (int)(info.Expiry - DateTime.UnixEpoch).TotalSeconds
        });

        await _table.PutItemAsync(doc);
    }

    public async Task DeleteAsync(string connectionId) =>
        await _table.DeleteItemAsync(connectionId);
}

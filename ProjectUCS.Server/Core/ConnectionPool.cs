using System.Net.Sockets;
using ProjectUCS.Common;

namespace ProjectUCS.Server.Core;

public class ConnectionPool
{
    private readonly Dictionary<Guid, Connection> _connections = new();
    public event EventHandler<Connection>? OnConnectionCreated;
    public event EventHandler<Connection>? OnConnectionRemoved;

    public void Add(Socket socket)
    {
        var connection = new Connection(socket);
        connection.OnDisconnected += (_, _) => Remove(connection);

        _connections.Add(connection.Id, connection);
        OnConnectionCreated?.Invoke(this, connection);
    }

    private void Remove(Connection connection)
    {
        _connections.Remove(connection.Id);
        OnConnectionRemoved?.Invoke(this, connection);
    }

    public Connection Get(Guid id)
    {
        return _connections[id];
    }

    public void RemoveAll()
    {
        _connections.Clear();
    }
}
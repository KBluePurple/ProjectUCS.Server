using System.Net.Sockets;

namespace ProjectUCS.Common;

public class ConnectionPool
{
    public event EventHandler<Connection>? OnConnectionCreated;
    public event EventHandler<Connection>? OnConnectionRemoved;
    
    private readonly Dictionary<Guid, Connection> _connections = new();

    public void Add(Socket socket)
    {
        var connection = new Connection(this, socket);
        _connections.Add(connection.Id, connection);
        OnConnectionCreated?.Invoke(this, connection);
    }
    
    public void Remove(Connection connection)
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
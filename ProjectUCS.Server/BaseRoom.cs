using ProjectUCS.Common;
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.RpcHandler;

namespace ProjectUCS.Server.Room;

public class BaseRoom : RpcHandler
{
    public BaseRoom(int maxPlayers = 2)
    {
        _maxPlayers = maxPlayers;
    }

    protected readonly List<Connection> _connections = new();
    private readonly int _maxPlayers;

    public int PlayerCount => _connections.Count;
    public int MaxPlayers => _maxPlayers;
    public bool IsFull => _connections.Count >= _maxPlayers;
    public IEnumerable<Connection> Players => _connections;

    public virtual void AddPlayer(Connection connection)
    {
        if (_connections.Count >= _maxPlayers)
            throw new Exception("Room is full!");

        foreach (var player in _connections)
            connection.Send(new S2C.Room.PlayerJoinedPacket { UserId = player.Id });

        _connections.Add(connection);
        connection.OnDisconnected += (_, _) => RemovePlayer(connection);

        Broadcast(new S2C.Room.PlayerJoinedPacket { UserId = connection.Id });
    }

    public virtual void RemovePlayer(Connection connection)
    {
        if (!_connections.Contains(connection))
            return;

        _connections.Remove(connection);
        Broadcast(new S2C.Room.PlayerLeftPacket { UserId = connection.Id });
    }
    
    public virtual void RemoveAllPlayers()
    {   
        _connections.Clear();
    }

    public virtual void Broadcast<T>(T packet) where T : IPacket
    {
        foreach (var connection in _connections) connection.Send(packet);
    }
}
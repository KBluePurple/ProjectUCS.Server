﻿using ProjectUCS.Common;
using ProjectUCS.Common.Data;

namespace ProjectUCS.Server.Room;

public class BaseRoom
{
    private readonly List<Connection> _connections = new();
    private readonly int _maxPlayers;

    public BaseRoom(int maxPlayers = 4)
    {
        _maxPlayers = maxPlayers;
    }

    public void AddPlayer(Connection connection)
    {
        if (_connections.Count >= _maxPlayers)
            throw new Exception("Room is full!");

        _connections.Add(connection);
        connection.OnDisconnected += (_, _) => RemovePlayer(connection);
        connection.OnPacketReceived += (_, packet) => Broadcast(packet);

        foreach (var player in _connections)
            connection.Send(new S2C.Room.PlayerJoinedPacket { UserId = player.Id });
        
        Broadcast(new S2C.Room.PlayerJoinedPacket { UserId = connection.Id });
    }

    public void RemovePlayer(Connection connection)
    {
        if (!_connections.Contains(connection))
            throw new Exception("Player is not in the room!");
        
        Broadcast(new S2C.Room.PlayerLeftPacket { UserId = connection.Id });

        _connections.Remove(connection);
    }

    public void Broadcast<T>(T packet) where T : class, IPacket
    {
        foreach (var connection in _connections) connection.Send(packet);
    }
}
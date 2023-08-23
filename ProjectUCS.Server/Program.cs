﻿using System.Reflection;
using ProjectUCS.Common;
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.RpcHandler;
using ProjectUCS.Server.Core;
using ProjectUCS.Server.Room;

// ReSharper disable UnusedMember.Local

PacketHandlerManager.RegisterHandlers(Assembly.GetExecutingAssembly());

MatchingManager.Initialize();

var protocol = new Protocol(7777);
protocol.Start();

protocol.OnClientConnected += (_, connection) =>
{
    Console.WriteLine("Client connected!");
    var welcomePacket = new S2C.WelcomePacket
    {
        UserId = connection.Id
    };
    connection.Send(welcomePacket);
};

protocol.OnClientDisconnected += (_, _) => { Console.WriteLine("Client disconnected!"); };

await Task.Delay(-1);

public class MatchingManager : RpcHandler
{
    private static readonly Matching Matching = new();
    private static MatchingManager? _instance;

    public static void Initialize()
    {
        _instance = new MatchingManager();
    }

    [RpcHandler(typeof(C2S.StartMatchPacket))]
    private void StartMatch(Connection connection, C2S.StartMatchPacket packet)
    {
        connection.Send(new S2C.MatchingStartedPacket());
        Matching.AddPlayer(connection);
    }
    
    [RpcHandler(typeof(C2S.CancelMatchPacket))]
    private void CancelMatch(Connection connection, C2S.CancelMatchPacket packet)
    {
        connection.Send(new S2C.MatchingStoppedPacket());
        Matching.RemovePlayer(connection);
    }
}

public class Matching : BaseRoom
{
    public override void AddPlayer(Connection connection)
    {
        base.AddPlayer(connection);
        connection.Send(new S2C.MatchInfoPacket
        {
            CurrentPlayers = PlayerCount,
            MaxPlayers = MaxPlayers
        });

        if (!IsFull) return;
        Broadcast(new S2C.MatchingEndedPacket());
        MakeGameRoom();
        RemoveAllPlayers();
    }

    private void MakeGameRoom()
    {
        var room = new GameRoom();
        foreach (var player in Players)
        {
            room.AddPlayer(player);
        }
    }
}

public class GameRoom : BaseRoom
{
    public override void AddPlayer(Connection connection)
    {
        connection.Send(new S2C.Room.RoomWelcomePacket());
        base.AddPlayer(connection);
    }
}
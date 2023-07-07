using System.Reflection;
using ProjectUCS.Common;
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.RpcHandler;
using ProjectUCS.Server.Core;
using ProjectUCS.Server.Room;
// ReSharper disable UnusedMember.Local

PacketHandlerManager.RegisterHandlers(Assembly.GetExecutingAssembly());

var helloPacket = new S2C.ChatPacket
{
    Message = "Hello!"
};

var protocol = new Protocol(7777);
protocol.Start();

var square = new BaseRoom(10);

protocol.OnClientConnected += (_, connection) =>
{
    Console.WriteLine("Client connected!");
    var welcomePacket = new S2C.WelcomePacket
    {
        UserId = connection.Id,
    };
    connection.Send(welcomePacket);
    square.AddPlayer(connection);
};

protocol.OnClientDisconnected += (_, _) => { Console.WriteLine("Client disconnected!"); };

await Task.Delay(-1);

internal class Test : RpcHandler
{
    [RpcHandler(typeof(C2S.ChatPacket))]
    private void OnChatPacket(Connection connection, C2S.ChatPacket packet)
    {
        Console.WriteLine($"Message: {packet.Message}");
        connection.Send(packet);
    }
    
    [RpcHandler(typeof(C2S.Room.MovePacket))]
    private void OnMovePacket(Connection connection, C2S.Room.MovePacket packet)
    {
        Console.WriteLine($"Move: {packet.Position.X}, {packet.Position.Y}");
        var movePacket = new S2C.Room.MovePacket
        {
            UserId = connection.Id,
            Position = packet.Position
        };
        connection.Send(movePacket);
    }
}
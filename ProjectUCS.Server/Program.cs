using System.Reflection;
using ProjectUCS.Common;
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.RpcHandler;
using ProjectUCS.Server.Core;
using ProjectUCS.Server.Room;

PacketHandlerManager.RegisterHandlers(Assembly.GetExecutingAssembly());

var protocol = new Protocol(7777);
protocol.Start();

var square = new BaseRoom(10);

protocol.OnClientConnected += (_, connection) =>
{
    Console.WriteLine("Client connected!");
    square.AddPlayer(connection);
};

protocol.OnClientDisconnected += (_, _) => { Console.WriteLine("Client disconnected!"); };

await Task.Delay(-1);

[UseRpc]
class Test
{
    [RpcHandler(typeof(C2S.ChatPacket))]
    void OnChatPacket(Connection connection, C2S.ChatPacket packet)
    {
        Console.WriteLine($"Message: {packet.Message}");
        connection.Send(packet);
    }
}
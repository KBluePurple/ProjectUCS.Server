using ProjectUCS.Common.Data;
using ProjectUCS.Server.Core;

PacketHandlerManager.RegisterHandlers();

var packet = new C2S.ChatPacket
{
    Message = "Hello World!"
};

var protocol = new Protocol(7777);
protocol.Start();

protocol.OnClientConnected += (_, connection) =>
{
    Console.WriteLine("Client connected!");
    connection.Send(packet);
};

protocol.OnClientDisconnected += (_, connection) =>
{
    Console.WriteLine("Client disconnected!");
};

await Task.Delay(-1);
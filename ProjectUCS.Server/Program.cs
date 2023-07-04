using System.Reflection;
using ProjectUCS.Common.Data;
using ProjectUCS.Server.Core;
using ProjectUCS.Server.Room;

PacketHandlerManager.RegisterHandlers(Assembly.GetExecutingAssembly());

var packet = new S2C.ChatPacket
{
    Message = "Hello World!"
};

var protocol = new Protocol(7777);
protocol.Start();

var square = new BaseRoom(10);

protocol.OnClientConnected += (_, connection) =>
{
    Console.WriteLine("Client connected!");
    connection.Send(packet);
    square.AddPlayer(connection);
};

protocol.OnClientDisconnected += (_, _) => { Console.WriteLine("Client disconnected!"); };

await Task.Delay(-1);
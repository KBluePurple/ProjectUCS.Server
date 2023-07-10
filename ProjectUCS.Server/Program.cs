using System.Reflection;
using ProjectUCS.Common.Data;
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

var square = new BaseRoom(100);

protocol.OnClientConnected += (_, connection) =>
{
    Console.WriteLine("Client connected!");
    var welcomePacket = new S2C.WelcomePacket
    {
        UserId = connection.Id
    };
    connection.Send(welcomePacket);
    square.AddPlayer(connection);
};

protocol.OnClientDisconnected += (_, _) => { Console.WriteLine("Client disconnected!"); };

await Task.Delay(-1);
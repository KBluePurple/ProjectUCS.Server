using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ProjectUCS.Common;
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.RpcHandler;

namespace ClientTest
{
    internal class Program
    {
        private Socket socket;

        private static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
            Task.Delay(-1).Wait();
        }

        private async void Run()
        {
            var moveManager = new MoveManager();
            
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777));
            
            Console.WriteLine($"Connected: {socket.Connected}");
            
            var connection = new Connection(socket);

            connection.OnPacketReceived += (_, packet) => { Console.WriteLine(packet.GetType()); };
        }
    }

    public class MoveManager : RpcHandler
    {
        [RpcHandler(typeof(S2C.ChatPacket))]
        private void OnChatPacket(Connection connection, S2C.ChatPacket packet)
        {
            Console.WriteLine($"Message: {packet.Message}");
        }
    }
}
using ProjectUCS.Common;
using ProjectUCS.Common.Data;

namespace ProjectUCS.Server.Core.Handlers;

[PacketHandler]
public class ChatPacketHandler : PacketHandler, IPacketHandler<C2S.ChatPacket>
{
    public void Handle(Connection connection, C2S.ChatPacket packet)
    {
        Console.WriteLine(packet.Message);
    }
}
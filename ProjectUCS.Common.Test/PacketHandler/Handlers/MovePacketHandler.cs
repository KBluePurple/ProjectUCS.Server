using ProjectUCS.Common.Data;

namespace ProjectUCS.Common.Test;

[PacketHandler]
public class MovePacketHandler : PacketHandler, IPacketHandler<C2S.MovePacket>
{
    public void Handle(Connection connection, C2S.MovePacket packet)
    {
        PacketHandlerTest.Handled = true;
    }
}
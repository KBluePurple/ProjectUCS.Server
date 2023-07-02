using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common.Test;

public class PacketHandlerTest
{
    public static bool Handled;

    [Test]
    public void GetPacketTest()
    {
        var packet = new C2S.ChatPacket
        {
            Message = "Test"
        };

        var root = PacketSerializer.Serialize(packet);

        PacketHandlerManager.Handle(null!, PacketSerializer.Deserialize(root));

        Assert.That(Handled, Is.True);
    }
}

[PacketHandler]
public class ChatPacketHandler : PacketHandler, IPacketHandler<C2S.ChatPacket>
{
    public void Handle(Connection connection, C2S.ChatPacket packet)
    {
        PacketHandlerTest.Handled = true;
    }
}
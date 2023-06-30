using System.Reflection;
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common.Test;

public class PacketHandlerTest
{
    public static bool Handled;
    
    [SetUp]
    public void Setup()
    {
        PacketHandlerManager.RegisterHandlers(Assembly.GetExecutingAssembly());
    }

    [Test]
    public void GetPacketTest()
    {
        var packet = new C2S.ChatPacket
        {
            Message = "Test"
        };

        var root = new RootPacket
        {
            Id = packet.GetType().GetHashCode(),
            Data = PacketSerializer.Serialize(packet)
        };

        PacketHandlerManager.Handle(null!, root);

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
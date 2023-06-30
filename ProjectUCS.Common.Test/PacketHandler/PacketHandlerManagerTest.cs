using ProjectUCS.Common.Data;

namespace ProjectUCS.Common.Test;

public class PacketHandlerManagerTest
{
    [Test]
    public void RegisterHandlersTest()
    {
        PacketHandlerManager.RegisterHandlers(GetType().Assembly);
        Assert.That(PacketHandlerManager.HandlerCount, Is.EqualTo(2));
    }
}
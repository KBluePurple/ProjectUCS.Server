using System.Reflection;
using ProjectUCS.Common.Data;

namespace ProjectUCS.Common.Test;

public class PacketHandlerManagerTest
{
    [Test, Order(1)]
    public void InitTest()
    {
        PacketHandlerManager.RegisterHandlers(Assembly.GetExecutingAssembly());
        Assert.That(PacketHandlerManager.HandlerCount, Is.EqualTo(2));
    }
}
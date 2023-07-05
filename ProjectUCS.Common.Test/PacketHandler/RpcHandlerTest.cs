using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.RpcHandler;

namespace ProjectUCS.Common.Test;

public class RpcHandlerTest
{
    public static bool IsCalled;

    [Test]
    public void HandleTest()
    {
        var packet = new C2S.ChatPacket
        {
            Message = "Hello World!"
        };

        var test = new Test();
        RpcHandleManager.Handle(null!, packet);
        test.Dispose();

        Assert.That(IsCalled, Is.True);
    }
}

internal class Test : RpcHandler
{
    [RpcHandler(typeof(C2S.ChatPacket))]
    private void OnChatPacket(Connection connection, C2S.ChatPacket packet)
    {
        RpcHandlerTest.IsCalled = true;
    }
}
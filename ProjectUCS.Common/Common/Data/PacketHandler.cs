using System.Reflection;
using MessagePack;

namespace ProjectUCS.Common.Data;

public class PacketHandler
{
    private readonly MethodInfo _handleMethod;
    private MethodInfo _deserializeMethod = null!;

    protected PacketHandler()
    {
        var type = GetType();
        _handleMethod = type.GetMethod("Handle", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    public void Init(Type packetType)
    {   
        var type = typeof(MessagePackSerializer);
        var method = type.GetMethod("Deserialize", BindingFlags.Static | BindingFlags.Public)!;
        _deserializeMethod = method.MakeGenericMethod(packetType);
    }

    public void HandleRoot(Connection connection, RootPacket root)
    {
        var packet = GetPacket(root);
        _handleMethod.Invoke(this, new[] { connection, packet });
    }

    private object GetPacket(RootPacket root)
    {
        var packet = _deserializeMethod.Invoke(null, new object[] { root.Data })!;
        return packet;
    }
}
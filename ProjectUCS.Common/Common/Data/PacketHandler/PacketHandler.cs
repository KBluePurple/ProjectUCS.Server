using System.Reflection;
using MessagePack;

namespace ProjectUCS.Common.Data;

public class PacketHandler
{
    private readonly MethodInfo _handleMethod;
    private Type _type;

    protected PacketHandler()
    {
        _type = GetType();
        _handleMethod = _type.GetMethod("Handle", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    public void Init(Type packetType)
    {
        _type = typeof(MessagePackSerializer);
    }

    public void HandleRoot(Connection connection, RootPacket root)
    {
        var packet = GetPacket(root);
        _handleMethod.Invoke(this, new[] { connection, packet });
    }

    private object GetPacket(RootPacket root)
    {
        var packet = MessagePackSerializer.Deserialize(_type, root.Data);
        return packet ?? throw new NullReferenceException();
    }
}
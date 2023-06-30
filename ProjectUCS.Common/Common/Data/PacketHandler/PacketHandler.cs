using System.Reflection;
using MessagePack;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common.Data;

public abstract class PacketHandler
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
        _type = packetType;
    }

    public void HandleRoot(Connection connection, RootPacket root)
    {
        var packet = GetPacket(root);
        _handleMethod.Invoke(this, new[] { connection, packet });
    }

    private object GetPacket(RootPacket root)
    {
        var packet = PacketSerializer.Deserialize(root.Data, _type);
        return packet ?? throw new NullReferenceException();
    }
}
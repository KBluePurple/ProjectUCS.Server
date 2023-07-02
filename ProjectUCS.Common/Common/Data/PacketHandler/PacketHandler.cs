using System.Reflection;
using MessagePack;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common.Data;

public abstract class PacketHandler
{
    private readonly MethodInfo _handleMethod;

    protected PacketHandler()
    {
        PacketType = GetType().GetInterfaces()
            .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IPacketHandler<>))
            .GetGenericArguments()[0];
        
        _handleMethod = GetType().GetMethod("Handle", BindingFlags.Instance | BindingFlags.Public)!;
        if (_handleMethod == null) throw new NullReferenceException();
    }

    public Type PacketType { get; private set; }

    public void HandleRoot(Connection connection, RootPacket root)
    {
        var packet = GetPacket(root);
        _handleMethod.Invoke(this, new[] { connection, packet });
    }

    private object GetPacket(RootPacket root)
    {
        var packet = PacketSerializer.Deserialize(root.Data, PacketType);
        return packet ?? throw new NullReferenceException();
    }
}
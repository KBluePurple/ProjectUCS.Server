using MessagePack;

namespace ProjectUCS.Common.Data.Serializer;

public static class PacketSerializer
{
    public static byte[] Serialize<TPacket>(TPacket packet) where TPacket : class, IPacket
    {
        var data = MessagePackSerializer.Serialize(packet);
        var root = new RootPacket
        {
            Id = packet.GetType().GetHashCode(),
            Data = data
        };
        return Serialize(root);
    }

    private static byte[] Serialize(RootPacket root)
    {
        return MessagePackSerializer.Serialize(root);
    }

    public static RootPacket Deserialize(byte[] data)
    {
        return MessagePackSerializer.Deserialize<RootPacket>(data);
    }
    
    public static object? Deserialize(byte[] data, Type type)
    {
        return MessagePackSerializer.Deserialize(type, data);
    }
}
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

    public static IPacket Deserialize(byte[] data)
    {
        var root = MessagePackSerializer.Deserialize<RootPacket>(data);
        return Deserialize(root.Data, root.Id);
    }

    public static IPacket Deserialize(byte[] data, int id)
    {
        var type = PacketManager.GetPacketType(id);
        return (IPacket?)MessagePackSerializer.Deserialize(type, data) ?? throw new InvalidOperationException();
    }

    public static RootPacket DeserializeRoot(byte[] data)
    {
        return MessagePackSerializer.Deserialize<RootPacket>(data);
    }
}
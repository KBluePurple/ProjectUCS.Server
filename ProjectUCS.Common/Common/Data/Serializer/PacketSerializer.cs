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
        return MessagePackSerializer.Serialize(root);
    }

    public static RootPacket Deserialize(byte[] data)
    {
        return MessagePackSerializer.Deserialize<RootPacket>(data);
    }
}
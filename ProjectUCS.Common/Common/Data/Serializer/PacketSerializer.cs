using System;
using MessagePack;

namespace ProjectUCS.Common.Data.Serializer
{
    public static class PacketSerializer
    {
        private static readonly object Lock = new object();
        
        public static byte[] Serialize<TPacket>(TPacket packet) where TPacket : IPacket
        {
            lock (Lock)
            {
                var data = MessagePackSerializer.Serialize(packet);
                var root = new RootPacket
                {
                    Id = packet.GetType().GetHash(),
                    Data = data
                };
                return Serialize(root);                
            }
        }

        private static byte[] Serialize(RootPacket root)
        {
            lock (Lock)
            {
                return MessagePackSerializer.Serialize(root);
            }
        }

        public static IPacket Deserialize(byte[] data)
        {
            lock (Lock)
            {
                var root = MessagePackSerializer.Deserialize<RootPacket>(data);
                return Deserialize(root.Data, root.Id);
            }
        }

        public static IPacket Deserialize(byte[] data, int id)
        {
            lock (Lock)
            {
                var type = PacketManager.GetPacketType(id);
                return (IPacket)MessagePackSerializer.Deserialize(type, data) ?? throw new InvalidOperationException();
            }
        }

        public static RootPacket DeserializeRoot(byte[] data)
        {
            lock (Lock)
            {
                return MessagePackSerializer.Deserialize<RootPacket>(data);
            }
        }
    }
}
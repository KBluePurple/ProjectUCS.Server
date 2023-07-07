namespace ProjectUCS.Common.Data
{
    public interface IPacketHandler<in TPacket> where TPacket : struct
    {
        void Handle(Connection connection, TPacket packet);
    }
}
namespace ProjectUCS.Common.Data
{
    public interface IPacketHandler<in TPacket> where TPacket : class
    {
        void Handle(Connection connection, TPacket packet);
    }
}
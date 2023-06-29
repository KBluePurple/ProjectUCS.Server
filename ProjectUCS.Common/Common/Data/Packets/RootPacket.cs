// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable CS8618

using MessagePack;

namespace ProjectUCS.Common.Data;

[MessagePackObject]
public class RootPacket
{
    [Key(0)] public int Id { get; set; }

    [Key(1)] public byte[] Data { get; set; }

    public void Handle(Connection connection)
    {
        PacketHandlerManager.Handle(connection, this);
    }
}

public static class C2S
{
    [MessagePackObject]
    public class ChatPacket : IPacket
    {
        [Key(0)] public string Message { get; set; }
    }
}
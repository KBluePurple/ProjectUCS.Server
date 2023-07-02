using MessagePack;
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common.Test;

public class PacketSerializerTest
{
    private readonly C2S.ChatPacket _packet = new()
    {
        Message = "Hello World!"
    };

    [Test]
    public void SerializeAndDeserializeTest()
    {
        var data = PacketSerializer.Serialize(_packet);
        var root = PacketSerializer.Deserialize(data);
        
        var packet = PacketSerializer.Deserialize(root.Data, _packet.GetType());

        Assert.Multiple(() =>
        {
            Assert.That(root.Id, Is.EqualTo(_packet.GetType().GetHashCode()));
            Assert.That(root.Data, Is.EqualTo(MessagePackSerializer.Serialize(_packet)));
            Assert.That(data, Is.EqualTo(MessagePackSerializer.Serialize(root)));
            Assert.That(MessagePackSerializer.Serialize(_packet), Is.EqualTo(MessagePackSerializer.Serialize(packet)));
        });
    }
}
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
    
    private readonly S2C.ChatPacket _helloPacket = new() 
    {
        Message = "Hello!"
    };

    [Test]
    public void SerializeAndDeserializeTest()
    {
        var data = PacketSerializer.Serialize(_packet);
        var root = PacketSerializer.DeserializeRoot(data);
        var packet = PacketSerializer.DeserializeRoot(data);
        Console.WriteLine($"{root.Id} - ({data.Length}){Convert.ToBase64String(data)}");
        
        var data2 = PacketSerializer.Serialize(_helloPacket);
        var root2 = PacketSerializer.DeserializeRoot(data2);
        var packet2 = PacketSerializer.DeserializeRoot(data2);
        Console.WriteLine($"{root.Id} - ({data2.Length}){Convert.ToBase64String(data2)}");

        Assert.Multiple(() =>
        {
            Assert.That(root.Id, Is.EqualTo(_packet.GetType().GetHash()));
            Assert.That(root.Data, Is.EqualTo(MessagePackSerializer.Serialize(_packet)));
            Assert.That(data, Is.EqualTo(MessagePackSerializer.Serialize(root)));
            Assert.That(MessagePackSerializer.Serialize(_packet), Is.EqualTo(packet.Data));
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(root2.Id, Is.EqualTo(_helloPacket.GetType().GetHash()));
            Assert.That(root2.Data, Is.EqualTo(MessagePackSerializer.Serialize(_helloPacket)));
            Assert.That(data2, Is.EqualTo(MessagePackSerializer.Serialize(root2)));
            Assert.That(MessagePackSerializer.Serialize(_helloPacket), Is.EqualTo(packet2.Data));
        });

        var packetData =
            Convert.FromBase64String(
                "ks4VO13RxDKS2SQwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDCSyj+AAADKP4AAAA==");
        
        for (var i = 0; i < 100000; i++)
        {
            var deserializedPacket = PacketSerializer.Deserialize(packetData);
            Console.WriteLine($"{deserializedPacket.GetType()}");
        }
    }
}
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common.Test;

public class PacketBuilderTest
{
    private int _susses = 0;
    
    [Test]
    public void BuildTest()
    {
        var packet = new C2S.ChatPacket
        {
            Message = "Test"
        };

        var serialized = PacketSerializer.Serialize(packet);
        var builder = new PacketBuilder();

        builder.OnCompleted += buffer =>
        {
            Assert.That(serialized, Is.EqualTo(buffer));
            _susses++;
        };

        builder.Init((uint)serialized.Length);
        for (var i = 0; i < serialized.Length; i += 2) builder.Append(serialized, (uint)i, 2);
        Assert.That(_susses, Is.EqualTo(1));
        
        builder.Init((uint)serialized.Length);
        for (var i = 0; i < serialized.Length; i += 2) builder.Append(serialized, (uint)i, 2);
        Assert.That(_susses, Is.EqualTo(2));
    }
}
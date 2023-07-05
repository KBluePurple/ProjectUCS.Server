using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common.Test;

public class PacketBuilderTest
{
    private bool _completed;
    
    [Test]
    public void BuildTest()
    {
        var packet = new C2S.ChatPacket
        {
            Message = "Test"
        };

        var serialized = PacketSerializer.Serialize(packet);
        var builder = new PacketBuilder();
        builder.Init(serialized.Length);

        builder.OnCompleted += buffer =>
        {
            var root = PacketSerializer.DeserializeRoot(buffer);
            _completed = true;
            Assert.That(root, Is.Not.Null);
        };

        for (var i = 0; i < serialized.Length; i++) builder.Append(serialized, i, 1);
        
        Assert.That(_completed, Is.True);
    }
}
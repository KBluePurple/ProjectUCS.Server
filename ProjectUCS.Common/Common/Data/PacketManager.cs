using System.Reflection;

namespace ProjectUCS.Common.Data;

public static class PacketManager
{
    static PacketManager()
    {
        RegisterPackets(Assembly.GetExecutingAssembly());
    }
    
    private static readonly Dictionary<int, Type> Packets = new();

    private static void RegisterPackets(Assembly assembly)
    {
        var types = assembly.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IPacket)));

        foreach (var type in types) Packets.Add(type.GetHashCode(), type);
    }

    public static Type GetPacketType(int id)
    {
        if (!Packets.TryGetValue(id, out var type))
            throw new Exception($"Packet with id {id} not found!");

        return type;
    }
}
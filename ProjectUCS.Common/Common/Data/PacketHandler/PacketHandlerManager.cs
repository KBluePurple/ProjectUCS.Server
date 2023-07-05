using System.Reflection;

namespace ProjectUCS.Common.Data;

public static class PacketHandlerManager
{
    public static int HandlerCount => Handlers.Count;
    private static readonly Dictionary<int, PacketHandler> Handlers = new();

    public static void RegisterHandlers(Assembly assembly)
    {
        InitHandlers(assembly);
    }

    private static void InitHandlers(Assembly assembly)
    {
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<PacketHandlerAttribute>();
            if (attribute == null) continue;
            if (Activator.CreateInstance(type) is not PacketHandler packetHandler) continue;
            if (Handlers.ContainsKey(packetHandler.PacketType.GetHashCode())) continue;
            Handlers.Add(packetHandler.PacketType.GetHashCode(), packetHandler);
        }
    }

    public static void Handle(Connection connection, IPacket packet)
    {
        if (!Handlers.TryGetValue(packet.GetType().GetHashCode(), out var handler)) return;
        handler.HandlePacket(connection, packet);
    }
}
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
            
            var packetHandler = Activator.CreateInstance(type) as PacketHandler;
            
            if (packetHandler == null) continue;
            
            var handlerType = type.GetInterfaces()
                .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IPacketHandler<>));
            
            var packetType = handlerType.GetGenericArguments()[0];
            packetHandler.Init(packetType);
            
            Handlers.Add(type.GetHashCode(), packetHandler);
        }
    }

    public static void Handle(Connection connection, RootPacket rootPacket)
    {
        if (!Handlers.TryGetValue(rootPacket.Id, out var handler)) return;
        handler.HandleRoot(connection, rootPacket);
    }
}
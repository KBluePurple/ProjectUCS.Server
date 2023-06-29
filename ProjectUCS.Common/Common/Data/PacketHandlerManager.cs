using System.Reflection;

namespace ProjectUCS.Common.Data;

public static class PacketHandlerManager
{
    private static readonly Dictionary<int, PacketHandler> Handlers = new();

    public static void RegisterHandlers()
    {
        InitHandlers();
    }

    private static void InitHandlers()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
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
            
            Handlers.Add(handlerType.GetHashCode(), packetHandler);
        }
    }

    public static void Handle(Connection connection, RootPacket rootPacket)
    {
        if (!Handlers.TryGetValue(rootPacket.Id, out var handler)) return;
        handler.HandleRoot(connection, rootPacket);
    }
}
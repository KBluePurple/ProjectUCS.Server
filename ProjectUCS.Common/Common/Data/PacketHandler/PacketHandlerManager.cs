using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProjectUCS.Common.Data
{
    public static class PacketHandlerManager
    {
        private static readonly Dictionary<int, PacketHandler> Handlers = new Dictionary<int, PacketHandler>();
        public static int HandlerCount => Handlers.Count;

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
                if (!(Activator.CreateInstance(type) is PacketHandler packetHandler)) continue;
                if (Handlers.ContainsKey(packetHandler.PacketType.GetHash())) continue;
                Handlers.Add(packetHandler.PacketType.GetHash(), packetHandler);
            }
        }

        public static void Handle(Connection connection, IPacket packet)
        {
            if (!Handlers.TryGetValue(packet.GetType().GetHash(), out var handler)) return;
            handler.HandlePacket(connection, packet);
        }
    }
}
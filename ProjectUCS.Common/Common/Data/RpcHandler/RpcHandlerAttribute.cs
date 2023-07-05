using System.Reflection;

namespace ProjectUCS.Common.Data.RpcHandler;

[AttributeUsage(AttributeTargets.Class)]
public class UseRpcAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class RpcHandlerAttribute : Attribute
{
    public RpcHandlerAttribute(Type packetType)
    {
        PacketType = packetType;
    }

    public Type PacketType { get; }
}

public static class RpcHandleManager
{
    private static readonly Dictionary<Type, List<(MethodInfo method, RpcHandler instance)>> Events = new();
    public static int HandlerCount => Events.Count;

    public static void Handle(Connection connection, IPacket packet)
    {
        if (Events.TryGetValue(packet.GetType(), out var actions))
            actions.ForEach(x => x.method.Invoke(x.instance, new object[] { connection, packet }));
    }

    public static void Register(RpcHandlerAttribute attribute, (MethodInfo method, RpcHandler instance) handler)
    {
        if (Events.TryGetValue(attribute.PacketType, out var events))
            events.Add(handler);
        else
            Events.Add(attribute.PacketType, new List<(MethodInfo method, RpcHandler instance)> { handler });
    }

    public static void Unregister(RpcHandlerAttribute attribute, (MethodInfo method, RpcHandler instance) handler)
    {
        if (Events.TryGetValue(attribute.PacketType, out var events))
            events.Remove(handler);
    }
}

public abstract class RpcHandler : IDisposable
{
    private readonly Dictionary<Type, (MethodInfo, RpcHandler)> _handlers = new();

    protected RpcHandler()
    {
        var type = GetType();

        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(x => x.GetCustomAttribute<RpcHandlerAttribute>() != null);

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<RpcHandlerAttribute>();
            if (attribute == null) continue;

            _handlers.Add(attribute.PacketType, (method, this));
            RpcHandleManager.Register(attribute, _handlers[attribute.PacketType]);
        }
    }

    public void Dispose()
    {
        foreach (var (packetType, _) in _handlers)
            RpcHandleManager.Unregister(new RpcHandlerAttribute(packetType), _handlers[packetType]);
    }
}
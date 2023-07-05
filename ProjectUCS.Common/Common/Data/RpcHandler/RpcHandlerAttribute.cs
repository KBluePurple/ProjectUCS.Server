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
    private static readonly Dictionary<Type, List<Action<Connection, IPacket>>> Events = new();
    public static int HandlerCount => Events.Count;

    public static void Handle(Connection connection, IPacket packet)
    {
        if (Events.TryGetValue(packet.GetType(), out var actions))
            actions.ForEach(x => x(connection, packet));
    }

    public static void Register(RpcHandlerAttribute attribute, MethodInfo method, object instance)
    {
        var action =
            (Action<Connection, IPacket>)Delegate.CreateDelegate(typeof(Action<Connection, IPacket>), instance, method);
        if (Events.TryGetValue(attribute.PacketType, out var events))
            events.Add(action);
        else
            Events.Add(attribute.PacketType, new List<Action<Connection, IPacket>> { action });
    }

    public static void Unregister(RpcHandlerAttribute attribute, MethodInfo method, object instance)
    {
        var action =
            (Action<Connection, IPacket>)Delegate.CreateDelegate(typeof(Action<Connection, IPacket>), instance, method);
        if (Events.TryGetValue(attribute.PacketType, out var events))
            events.Remove(action);
    }
}

public abstract class RpcHandler : IDisposable
{
    private readonly Dictionary<Type, Action<Connection, IPacket>> _handlers = new();

    protected RpcHandler()
    {
        var type = GetType();

        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(x => x.GetCustomAttribute<RpcHandlerAttribute>() != null);

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<RpcHandlerAttribute>();
            if (attribute == null) continue;

            _handlers.Add(attribute.PacketType,
                (connection, packet) => method.Invoke(this, new object[] { connection, packet }));

            RpcHandleManager.Register(attribute, method, this);
        }
    }

    public void Dispose()
    {
        foreach (var (packetType, _) in _handlers)
            RpcHandleManager.Unregister(new RpcHandlerAttribute(packetType), GetType().GetMethod(packetType.Name)!,
                this);
    }

    private void Handle(Connection connection, IPacket packet)
    {
        if (_handlers.TryGetValue(packet.GetType(), out var handler))
            handler(connection, packet);
    }
}
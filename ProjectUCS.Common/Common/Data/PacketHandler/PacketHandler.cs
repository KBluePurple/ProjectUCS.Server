using System;
using System.Linq;
using System.Reflection;

namespace ProjectUCS.Common.Data
{
    public abstract class PacketHandler
    {
        private readonly MethodInfo _handleMethod;

        protected PacketHandler()
        {
            PacketType = GetType().GetInterfaces()
                .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IPacketHandler<>))
                .GetGenericArguments()[0];

            _handleMethod = GetType().GetMethod("Handle", BindingFlags.Instance | BindingFlags.Public);
            if (_handleMethod == null) throw new NullReferenceException();
        }

        public Type PacketType { get; private set; }

        public void HandlePacket(Connection connection, IPacket packet)
        {
            _handleMethod.Invoke(this, new object[] { connection, packet });
        }
    }
}
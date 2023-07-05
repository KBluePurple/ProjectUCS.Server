using System;
using System.Reflection;

namespace ProjectUCS.Common.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketHandlerAttribute : Attribute
    {
        public void Handle(Connection connection, RootPacket rootPacket)
        {
            var type = GetType();
            var method = type.GetMethod("Handle", BindingFlags.Instance | BindingFlags.NonPublic);
            method?.Invoke(this, new object[] { connection, rootPacket.Data });
        }
    }
}
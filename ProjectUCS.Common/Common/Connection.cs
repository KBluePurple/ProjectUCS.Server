using System;
using System.Net.Sockets;
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.RpcHandler;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common
{
    public class Connection
    {
        private readonly byte[] _buffer = new byte[1024];
        private readonly PacketBuilder _packetBuilder = new PacketBuilder();
        private readonly Socket _socket;

        public Connection(Socket socket)
        {
            _packetBuilder.OnCompleted += OnPacketCompleted;
            _socket = socket;

            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, Received, null);
        }

        public Guid Id { get; } = Guid.NewGuid();
        public bool IsAlive => _socket.Connected;

        private void Received(IAsyncResult ar)
        {
            var bytesTransferred = (uint)_socket.EndReceive(ar);
            uint offset = 0;

            Console.WriteLine($"Received: {Convert.ToBase64String(_buffer, 0, (int)bytesTransferred)}");

            if (bytesTransferred == 0)
            {
                Disconnect();
                return;
            }

            while (bytesTransferred - offset > 0)
            {
                if (_packetBuilder.IsNew)
                {
                    var packetSize = BitConverter.ToUInt32(_buffer, 0);
                    _packetBuilder.Init(packetSize);
                    offset += 4;
                    continue;
                }

                if (_packetBuilder.Offset + bytesTransferred - offset > _packetBuilder.Size)
                {
                    var count = _packetBuilder.Size - _packetBuilder.Offset;
                    _packetBuilder.Append(_buffer, offset, count);
                    offset += count;
                    continue;
                }
                
                _packetBuilder.Append(_buffer, offset, bytesTransferred - offset);
                offset += bytesTransferred;
            }
            
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, Received, null);
        }

        private void OnPacketCompleted(byte[] buffer)
        {
            var packet = PacketSerializer.Deserialize(buffer);
            Handled(packet);
            PacketHandlerManager.Handle(this, packet);
            RpcHandleManager.Handle(this, packet);
        }

        private void Disconnect()
        {
            _socket.Close();
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        }

        public void Send<T>(T packet) where T : class, IPacket
        {
            Send(PacketSerializer.Serialize(packet));
        }

        private void Send(byte[] buffer)
        {
            var size = BitConverter.GetBytes((uint)buffer.Length);
            _socket.BeginSend(size, 0, size.Length, SocketFlags.None, null, null);
            _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, null);

            Console.WriteLine($"Sent: {Convert.ToBase64String(buffer)}");
        }

        private void Handled<T>(T packet) where T : class, IPacket
        {
            OnPacketReceived?.Invoke(this, packet);
        }

        public event EventHandler OnDisconnected;
        public event EventHandler<IPacket> OnPacketReceived;
    }
}
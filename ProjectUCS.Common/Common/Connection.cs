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

        private void Received(IAsyncResult ar) // 나중에 정리할 코드
        {
            try
            {
                var bytesTransferred = _socket.EndReceive(ar);
                var offset = 0;

                if (bytesTransferred == 0)
                {
                    Disconnect();
                    return;
                }

                while (bytesTransferred - offset > 0)
                {
                    if (_packetBuilder.IsNew)
                    {
                        // 패킷 사이즈 정보가 잘리면 버그 발생
                        if (bytesTransferred - offset < 4) break;
                        var packetSize = (int)BitConverter.ToUInt32(_buffer, offset);
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
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}\n{e.StackTrace}");
                Disconnect();
            }
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

        public void Send<T>(T packet) where T : IPacket
        {
            var serialized = PacketSerializer.Serialize(packet);
            Send(serialized);
        }

        private void Send(byte[] buffer)
        {
            var size = BitConverter.GetBytes((uint)buffer.Length);
            _socket.BeginSend(size, 0, size.Length, SocketFlags.None, null, null);
            _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, null);
        }

        private void Handled<T>(T packet) where T : class, IPacket
        {
            OnPacketReceived?.Invoke(this, packet);
        }

        public event EventHandler OnDisconnected;
        public event EventHandler<IPacket> OnPacketReceived;
    }
}
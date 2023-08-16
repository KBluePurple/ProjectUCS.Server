using System;
using System.Net.Sockets;
using System.Threading;
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.RpcHandler;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common
{
    public class Connection
    {
        private readonly PacketBuilder _packetBuilder = new PacketBuilder();
        private readonly byte[] _receiveBuffer = new byte[1024];
        private readonly byte[] _sendBuffer = new byte[1024];
        private readonly Socket _socket;
        private int _readOffset;
        private int _writeOffset;

        public Connection(Socket socket)
        {
            _packetBuilder.OnCompleted += OnPacketCompleted;
            _socket = socket;

            socket.BeginReceive(_receiveBuffer, _writeOffset, _receiveBuffer.Length, SocketFlags.None, Received, null);
        }

        private int RemainSize => _writeOffset - _readOffset;

        public int Id { get; } = SessionManager.GetId();
        public bool IsAlive => _socket.Connected;

        private void Received(IAsyncResult ar)
        {
            try
            {
                ProcessPacket(_socket.EndReceive(ar));
                _socket.BeginReceive(_receiveBuffer, _writeOffset, _receiveBuffer.Length, SocketFlags.None, Received,
                    null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while receive packet: {e.Message}\n{e.StackTrace}");
                Disconnect();
            }
        }

        private void ProcessPacket(int bytesTransferred)
        {
            Interlocked.Add(ref _writeOffset, bytesTransferred);

            if (bytesTransferred == 0)
            {
                Disconnect();
                return;
            }

            while (RemainSize > 0)
            {
                // 이전 패킷이 완성되었을 때
                if (_packetBuilder.IsNew)
                {
                    // 패킷 빌더 초기화
                    if (RemainSize < 4)
                    {
                        break;
                    }

                    InitPacketBuilder();
                    Interlocked.Add(ref _readOffset, 4);
                }

                var count = Math.Min(_packetBuilder.Size - _packetBuilder.Offset, RemainSize);
                if (count == 0) break;
                _packetBuilder.Append(_receiveBuffer, _readOffset, count);
                Interlocked.Add(ref _readOffset, count);
            }

            Array.Copy(_receiveBuffer, _readOffset, _receiveBuffer, 0, RemainSize);

            _writeOffset = RemainSize;
            _readOffset = 0;
        }

        private void InitPacketBuilder()
        {
            var packetSize = (int)BitConverter.ToUInt32(_receiveBuffer, _readOffset);
            _packetBuilder.Init(packetSize);
        }

        private void OnPacketCompleted(byte[] buffer)
        {
            var packet = PacketSerializer.Deserialize(buffer);
            HandlePacket(packet);

            try
            {
                PacketHandlerManager.Handle(this, packet);
                RpcHandleManager.Handle(this, packet);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while handling packet: {e.Message}\n{e.StackTrace}");
            }
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
            try
            {
                var size = BitConverter.GetBytes((uint)buffer.Length);
                Array.Copy(size, 0, _sendBuffer, 0, size.Length);
                Array.Copy(buffer, 0, _sendBuffer, size.Length, buffer.Length);
                _socket.Send(_sendBuffer, 0, size.Length + buffer.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while sending packet: {e.Message}\n{e.StackTrace}");
                Disconnect();
            }
        }

        private void HandlePacket<T>(T packet) where T : class, IPacket
        {
            OnPacketReceived?.Invoke(this, packet);
        }

        public event EventHandler OnDisconnected;
        public event EventHandler<IPacket> OnPacketReceived;
    }

    public static class SessionManager
    {
        private static int _sessionCount;

        public static int GetId()
        {
            return _sessionCount++;
        }
    }
}
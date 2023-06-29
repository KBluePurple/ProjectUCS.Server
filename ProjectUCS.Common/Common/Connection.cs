using System.Net.Sockets;
using MessagePack;
using ProjectUCS.Common.Data;

namespace ProjectUCS.Common;

public class Connection
{
    private readonly ConnectionPool _connectionPool;
    private readonly PacketBuilder _packetBuilder = new();
    private readonly Socket _socket;

    public Connection(ConnectionPool connectionPool, Socket socket)
    {
        _connectionPool = connectionPool;
        _packetBuilder.OnCompleted += OnPacketCompleted;
        _socket = socket;

        var socketAsyncEventArgs = new SocketAsyncEventArgs();
        socketAsyncEventArgs.Completed += ReceiveCompleted;
        socket.ReceiveAsync(socketAsyncEventArgs);
    }

    public Guid Id { get; } = Guid.NewGuid();

    private void ReceiveCompleted(object? sender, SocketAsyncEventArgs e)
    {
        if (e.Buffer == null) return;

        if (_packetBuilder.IsNew)
        {
            var packetSize = BitConverter.ToInt32(e.Buffer, 0);
            _packetBuilder.Init(packetSize);
        }
        else
        {
            _packetBuilder.Append(e.Buffer, 0, e.BytesTransferred);
        }

        _socket.ReceiveAsync(e);
    }

    private void OnPacketCompleted(byte[] buffer)
    {
        var packet = MessagePackSerializer.Deserialize<RootPacket>(buffer);
        packet.Handle(this);
    }

    public void Disconnect()
    {
        _connectionPool.Remove(this);
        _socket.Close();
    }

    public void Send(IPacket packet)
    {
        var data = MessagePackSerializer.Serialize(packet);
        var root = new RootPacket
        {
            Id = packet.GetType().GetHashCode(),
            Data = data
        };
        var buffer = MessagePackSerializer.Serialize(root);

        var socketAsyncEventArgs = new SocketAsyncEventArgs();
        _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, socketAsyncEventArgs);
    }
}
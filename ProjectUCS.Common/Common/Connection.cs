using System.Net.Sockets;
using MessagePack;
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.Compress;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common;

public class Connection
{
    private readonly PacketBuilder _packetBuilder = new();
    private readonly Socket _socket;

    public Connection(Socket socket)
    {
        _packetBuilder.OnCompleted += OnPacketCompleted;
        _socket = socket;

        var socketAsyncEventArgs = new SocketAsyncEventArgs();
        socketAsyncEventArgs.Completed += ReceiveCompleted;
        socket.ReceiveAsync(socketAsyncEventArgs);
    }

    public Guid Id { get; } = Guid.NewGuid();
    public bool IsAlive => _socket.Connected;

    private void ReceiveCompleted(object? sender, SocketAsyncEventArgs e)
    {
        if (e.Buffer == null)
        {
            Disconnect();
            return;
        }

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
        // var uncompressed = DataCompressor.Decompress(buffer);
        var packet = MessagePackSerializer.Deserialize<RootPacket>(buffer);
        packet.Handle(this);
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
        // var compressed = DataCompressor.Compress(buffer);
        var socketAsyncEventArgs = new SocketAsyncEventArgs();
        _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, socketAsyncEventArgs);
    }

    public event EventHandler? OnDisconnected;
}
using System.Net.Sockets;
using ProjectUCS.Common.Data;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common;

public class Connection
{
    private const int BufferSize = 1024;
    private readonly PacketBuilder _packetBuilder = new();
    private readonly Socket _socket;

    public Connection(Socket socket)
    {
        _packetBuilder.OnCompleted += OnPacketCompleted;
        _socket = socket;

        var socketAsyncEventArgs = new SocketAsyncEventArgs();
        socketAsyncEventArgs.Completed += ReceiveCompleted;
        socketAsyncEventArgs.SetBuffer(new byte[BufferSize], 0, BufferSize);
        if (socketAsyncEventArgs.Buffer != null)
            socket.BeginReceive(socketAsyncEventArgs.Buffer, 0, socketAsyncEventArgs.Buffer.Length, SocketFlags.None,
                null, socketAsyncEventArgs);
    }

    public Guid Id { get; } = Guid.NewGuid();
    public bool IsAlive => _socket.Connected;

    private void ReceiveCompleted(object? sender, SocketAsyncEventArgs e)
    {
        if (e.Buffer == null || e.BytesTransferred == 0)
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

        _socket.BeginReceive(e.Buffer, 0, e.Buffer.Length, SocketFlags.None, null, e);
    }

    private void OnPacketCompleted(byte[] buffer)
    {
        var packet = PacketSerializer.Deserialize(buffer);
        PacketHandlerManager.Handle(this, packet);
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
        var socketAsyncEventArgs = new SocketAsyncEventArgs();
        _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, socketAsyncEventArgs);
    }
    
    public void Handled<T>(T packet) where T : class, IPacket
    {
        OnPacketReceived?.Invoke(this, packet);
    }

    public event EventHandler? OnDisconnected;
    public event EventHandler<IPacket>? OnPacketReceived;
}
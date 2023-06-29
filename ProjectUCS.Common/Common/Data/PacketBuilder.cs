namespace ProjectUCS.Common.Data;

public class PacketBuilder
{
    private byte[] _buffer;
    private int _offset;
    private int _size;

    public PacketBuilder()
    {
        _offset = 0;
        _size = 0;
        _buffer = Array.Empty<byte>();
    }

    public bool IsNew => _offset == 0;
    public bool IsCompleted => _offset == _size;
    public event Action<byte[]>? OnCompleted;

    public void Init(int packetSize)
    {
        _offset = 0;
        _size = packetSize;
        _buffer = new byte[packetSize];
    }

    public void Append(byte[] buffer, int offset, int count)
    {
        Array.Copy(buffer, offset, _buffer, _offset, count);
        _offset += count;

        if (_offset != _size) return;
        OnCompleted?.Invoke(_buffer);
    }
}
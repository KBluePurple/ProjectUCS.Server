using System;

namespace ProjectUCS.Common.Data
{
    public class PacketBuilder
    {
        private byte[] _buffer;
        private uint _offset;
        private uint _size;

        public PacketBuilder()
        {
            _offset = 0;
            _size = 0;
            _buffer = Array.Empty<byte>();
        }

        public bool IsNew { get; private set; } = true;
        public bool IsCompleted => _offset == _size;
        public uint Offset => _offset;
        public uint Size => _size;

        public event Action<byte[]> OnCompleted;

        public void Init(uint packetSize)
        {
            IsNew = false;
            _offset = 0;
            _size = packetSize;
            _buffer = new byte[packetSize];

            Console.WriteLine($"Packet size: {packetSize}");
        }

        public void Append(byte[] buffer, uint offset, uint count)
        {
            if (_offset + count > _size)
                throw new Exception("Invalid packet size.");

            Console.WriteLine($"Append: {count}");
            
            Array.Copy(buffer, offset, _buffer, _offset, count);
            _offset += count;

            if (_offset != _size) return;
            OnCompleted?.Invoke(_buffer);
            IsNew = true;
        }
    }
}
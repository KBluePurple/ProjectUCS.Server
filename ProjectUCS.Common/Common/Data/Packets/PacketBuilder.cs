using System;
using ProjectUCS.Common.Data.Serializer;

namespace ProjectUCS.Common.Data
{
    public class PacketBuilder
    {
        private byte[] _buffer;
        private int _offset;
        private int _size;
        private object _lock = new object();

        public PacketBuilder()
        {
            _offset = 0;
            _size = 0;
            _buffer = Array.Empty<byte>();
        }

        public bool IsNew { get; private set; } = true;
        public bool IsCompleted => _offset == _size;
        public int Offset => _offset;
        public int Size => _size;

        public event Action<byte[]> OnCompleted;

        public void Init(int packetSize)
        {
            lock (_lock)
            {
                IsNew = false;
                _offset = 0;
                _size = packetSize;
                _buffer = new byte[packetSize];
            }
        }

        public void Append(byte[] buffer, int offset, int count)
        {
            lock (_lock)
            {
                if (_offset + count > _size)
                    throw new Exception("Invalid packet size.");

                Array.Copy(buffer, offset, _buffer, _offset, count);
                _offset += count;

                if (_offset != _size) return;
                OnCompleted?.Invoke(_buffer);
                IsNew = true;
            }
        }
    }
}
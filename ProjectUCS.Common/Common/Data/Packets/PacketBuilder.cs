using System;

namespace ProjectUCS.Common.Data
{
    public class PacketBuilder
    {
        private readonly object _lock = new object();
        private byte[] _buffer;

        public PacketBuilder()
        {
            Offset = 0;
            Size = 0;
            _buffer = Array.Empty<byte>();
        }

        public bool IsNew { get; private set; } = true;
        public bool IsCompleted => Offset == Size;
        public int Offset { get; private set; }

        public int Size { get; private set; }

        public event Action<byte[]> OnCompleted;

        public void Init(int packetSize)
        {
            lock (_lock)
            {
                IsNew = false;
                Offset = 0;
                Size = packetSize;
                _buffer = new byte[packetSize];
                // Console.WriteLine($"PacketBuilder Init {packetSize}");
            }
        }

        public void Append(byte[] buffer, int offset, int count)
        {
            lock (_lock)
            {
                if (Offset + count > Size)
                    throw new Exception("Invalid packet size.");

                Array.Copy(buffer, offset, _buffer, Offset, count);
                Offset += count;

                // Console.WriteLine($"PacketBuilder Append {count}({offset})\n{Convert.ToBase64String(buffer.AsSpan(offset, count).ToArray())}");

                if (Offset != Size) return;
                OnCompleted?.Invoke(_buffer);
                IsNew = true;
            }
        }
    }
}
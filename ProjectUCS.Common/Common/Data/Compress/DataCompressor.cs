using ZstdSharp;

namespace ProjectUCS.Common.Data.Compress;

public static class DataCompressor
{
    const int Level = 1;
    
    public static byte[] Compress(byte[] data)
    {
        using var decompressor = new Compressor(Level);
        var compressed = decompressor.Wrap(data);
        return compressed.ToArray();
    }

    public static byte[] Decompress(byte[] data)
    {
        using var decompressor = new Decompressor();
        var decompressed = decompressor.Unwrap(data);
        return decompressed.ToArray();
    }
}
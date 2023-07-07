using System;
using System.Linq;
using System.Security.Cryptography;

public static class Hasher
{
    private static int Compute(Type type)
    {
        using (var md5 = MD5.Create())
        {
            var str = type.FullName ?? type.Name;
            var hash = md5.ComputeHash(str.Select(x => (byte)x).ToArray());
            return BitConverter.ToInt32(hash, 0);
        }
    }
    
    public static int GetHash(this Type type)
    {
        return Compute(type);
    }
}
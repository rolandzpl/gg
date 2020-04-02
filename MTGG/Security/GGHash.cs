using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace MTGG.Security
{
    public enum HashType : byte
    {
        GG32 = 0x01,
        SHA1 = 0x02
    }
 
    internal static class GGHash
    {
        public static byte[] SHA1(string pass, uint seed)
        {
            byte[] passBin = Encoding.UTF8.GetBytes(pass);
            byte[] seedBin = BitConverter.GetBytes(seed);
            List<Byte> mergedBin = new List<byte>();
            mergedBin.AddRange(passBin);
            mergedBin.AddRange(seedBin);
            SHA1 sha1 = SHA1Managed.Create();
            return sha1.ComputeHash(mergedBin.ToArray());
        }
 
        public static byte[] GG32(string password, uint seed)
        {
            uint x, y, z;
            y = seed;
            int p = 0;
            for (x = 0; p < password.Length; p++)
            {
                x = (x & 0xffffff00) | Convert.ToByte(password[p]);
                y ^= x;
                y += x;
                x <<= 8;
                y ^= x;
                x <<= 8;
                y -= x;
                x <<= 8;
                y ^= x;
 
                z = y & 0x1f;
                y = (uint)((uint)y << (int)z) | (uint)((uint)y >> (int)(32 - z));
            }
            return BitConverter.GetBytes(y);
        }
    }
}

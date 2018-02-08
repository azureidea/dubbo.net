using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dubbo.Net.Common
{
    public static class ByteUtil
    {
        public static void Short2Bytes(short v, byte[] b, int offset = 0)
        {
            var bV = unchecked ((byte)v);
            b[offset + 1] =bV;
            b[offset + 0] = (byte)(v >> 8);
        }

        public static byte[] Short2Bytes(short v)
        {
            var b = new byte[2];
            Short2Bytes(v, b);
            return b;
        }

        public static void Long2Bytes(long v, byte[] b, int off = 0)
        {
            b[off + 7] = (byte)v;
            b[off + 6] = (byte)(v >> 8);
            b[off + 5] = (byte)(v >> 16);
            b[off + 4] = (byte)(v >> 24);
            b[off + 3] = (byte)(v >> 32);
            b[off + 2] = (byte)(v >> 40);
            b[off + 1] = (byte)(v >> 48);
            b[off + 0] = (byte)(v >> 56);
        }
        public static void Int2Bytes(int v, byte[] b, int off=0)
        {
            b[off + 3] = (byte)v;
            b[off + 2] = (byte)(v >> 8);
            b[off + 1] = (byte)(v >> 16);
            b[off + 0] = (byte)(v >> 24);
        }

        public static int Bytes2Int(byte[] b, int off = 0)
        {
            return ((b[off + 3] & 0xFF) << 0) +
                ((b[off + 2] & 0xFF) << 8) +
                ((b[off + 1] & 0xFF) << 16) +
                ((b[off + 0]) << 24);
        }
        public static long Bytes2Long(byte[] b, int off)
        {
            return ((b[off + 7] & 0xFFL) << 0) +
                    ((b[off + 6] & 0xFFL) << 8) +
                    ((b[off + 5] & 0xFFL) << 16) +
                    ((b[off + 4] & 0xFFL) << 24) +
                    ((b[off + 3] & 0xFFL) << 32) +
                    ((b[off + 2] & 0xFFL) << 40) +
                    ((b[off + 1] & 0xFFL) << 48) +
                    (((long)b[off + 0]) << 56);
        }

        public static byte[] CopyOf(byte[] src, int length)
        {
            var dst = new byte[length];
            System.Array.Copy(src,0,dst,0,Math.Min(src.Length,length));
            return dst;
        }
    }
}

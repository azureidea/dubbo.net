using DotNetty.Buffers;
using System;

namespace Dubbo.Net.Remoting.Buffer
{
    public static class ChannelBuffers
    {
        public static readonly IChannelBuffer EmptyBuffer = new HeapChannelBuffer(0);


        //public static IChannelBuffer DynamicBuffer(int capacity = 256)
        //{
        //    return new DynamicChannelBuffer(capacity);
        //}

        //public static IChannelBuffer DynamicBuffer(int capacity,
        //                                          IChannelBufferFactory factory)
        //{
        //    return new DynamicChannelBuffer(capacity, factory);
        //}

        public static IChannelBuffer Buffer(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentException("capacity can not be negative");
            }
            if (capacity == 0)
            {
                return EmptyBuffer;
            }
            return new HeapChannelBuffer(capacity);
        }

        public static IChannelBuffer WrappedBuffer(byte[] array, int offset, int length)
        {
            if (array == null)
            {
                throw new NullReferenceException("array == null");
            }
            byte[] dest = new byte[length];
            Array.Copy(array, offset, dest, 0, length);
            return WrappedBuffer(dest);
        }

        public static IChannelBuffer WrappedBuffer(byte[] array)
        {
            if (array == null)
            {
                throw new NullReferenceException("array == null");
            }
            if (array.Length == 0)
            {
                return EmptyBuffer;
            }
            return new HeapChannelBuffer(array);
        }

        public static IChannelBuffer WrappedBuffer(IByteBuffer buffer)
        {
            if (!buffer.HasArray)
            {
                return EmptyBuffer;
            }
            if (buffer.HasArray)
            {
                return WrappedBuffer(buffer.Array, buffer.ArrayOffset + buffer.ReaderIndex, buffer.ReadableBytes);
            }
            else
            {
                return new ByteBufferBackedChannelBuffer(buffer);
            }
        }

        public static IChannelBuffer DirectBuffer(int capacity)
        {
            if (capacity == 0)
            {
                return EmptyBuffer;
            }

            IChannelBuffer buffer = new ByteBufferBackedChannelBuffer(
                    Unpooled.Buffer(capacity));
            buffer.Clear();
            return buffer;
        }

        public static bool Equals(IChannelBuffer bufferA, IChannelBuffer bufferB)
        {
             int aLen = bufferA.ReadableBytes;
            if (aLen != bufferB.ReadableBytes)
            {
                return false;
            }

             int byteCount = aLen & 7;

            int aIndex = bufferA.ReaderIndex;
            int bIndex = bufferB.ReaderIndex;

            for (int i = byteCount; i > 0; i--)
            {
                if (bufferA.GetByte(aIndex) != bufferB.GetByte(bIndex))
                {
                    return false;
                }
                aIndex++;
                bIndex++;
            }

            return true;
        }

        public static int Compare(IChannelBuffer bufferA, IChannelBuffer bufferB)
        {
             int aLen = bufferA.ReadableBytes;
             int bLen = bufferB.ReadableBytes;
             int minLength = Math.Min(aLen, bLen);

            int aIndex = bufferA.ReaderIndex;
            int bIndex = bufferB.ReaderIndex;

            for (int i = minLength; i > 0; i--)
            {
                byte va = bufferA.GetByte(aIndex);
                byte vb = bufferB.GetByte(bIndex);
                if (va > vb)
                {
                    return 1;
                }
                else if (va < vb)
                {
                    return -1;
                }
                aIndex++;
                bIndex++;
            }

            return aLen - bLen;
        }
    }
}

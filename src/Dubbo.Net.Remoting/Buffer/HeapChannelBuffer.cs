using System;
using System.IO;
using DotNetty.Buffers;

namespace Dubbo.Net.Remoting.Buffer
{
    public class HeapChannelBuffer:AbstractChannelBuffer
    {
        protected readonly byte[] _array;

        public HeapChannelBuffer(int length):this(new byte[length])
        {
        }

        public HeapChannelBuffer(byte[] array):this(array,0,array.Length)
        {

        }

        public HeapChannelBuffer(byte[] ary, int readerIndex, int writerIndex)
        {
            _array = ary ?? throw new NullReferenceException("array");
            SetIndex(readerIndex,writerIndex);
        }

        public override byte[] Array => _array;

        public override bool HasArray => true;

        public override int ArrayOffset => 0;

        public override void SetBytes(int index, IChannelBuffer src, int srcIndex, int length)
        {
            if (src is HeapChannelBuffer buffer) {
                SetBytes(index, buffer.Array, srcIndex, length);
            } else {
                src.GetBytes(srcIndex, _array, index, length);
            }
        }

        public override int SetBytes(int index, MemoryStream src, int length)
        {
            int readBytes = 0;
            do
            {
                int localReadBytes = src.Read(_array, index, length);
                if (localReadBytes < 0)
                {
                    if (readBytes == 0)
                    {
                        return -1;
                    }
                    else
                    {
                        break;
                    }
                }
                readBytes += localReadBytes;
                index += localReadBytes;
                length -= localReadBytes;
            } while (length > 0);

            return readBytes;
        }

        public override int Capacity => _array.Length;

        public override IChannelBuffer Copy(int index, int length)
        {
            if (index < 0 || length < 0 || index + length > _array.Length)
            {
                throw new IndexOutOfRangeException();
            }

            byte[] copiedArray = new byte[length];
            System.Array.Copy(_array, index, copiedArray, 0, length);
            return new HeapChannelBuffer(copiedArray);
        }

        public override IChannelBufferFactory Factory=> HeapChannelBufferFactory.GetInstance();

        public override byte GetByte(int index)
        {
            throw new NotImplementedException();
        }

        public override void GetBytes(int index, byte[] dst, int dstIndex, int length)
        {
            throw new NotImplementedException();
        }

        public override void GetBytes(int index, IByteBuffer dst)
        {
            dst.WriteBytes(_array, index, dst.WritableBytes);
        }

        public override IByteBuffer ToByteBuffer(int index, int length)
        {
            var buffer = Unpooled.Buffer(length);
            buffer.WriteBytes(_array, index, length);
            return buffer;
        }

        public override void WiteBytes(byte[] src)
        {
            throw new NotImplementedException();
        }

        public override void GetBytes(int index, IChannelBuffer dst, int dstIndex, int length)
        {
            throw new NotImplementedException();
        }

        public override void GetBytes(int index, MemoryStream dst, int length)
        {
            dst.Write(_array,index,length);
        }

        public override void SetByte(int index, int value)
        {
            _array[index] = (byte)value;
        }

        public override void SetBytes(int index, byte[] src, int srcIndex, int length)
        {
            System.Array.Copy(src,srcIndex,_array,index,length);
        }

        public override void SetBytes(int index, IByteBuffer src)
        {
            src.GetBytes(index, _array);
        }

        public override bool IsDirect => false;

    }
}

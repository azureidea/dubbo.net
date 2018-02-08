using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNetty.Buffers;

namespace Dubbo.Net.Remoting.Buffer
{
    public class ByteBufferBackedChannelBuffer:AbstractChannelBuffer
    {
        public  IByteBuffer Buffer { get; }
        
        public ByteBufferBackedChannelBuffer(IByteBuffer buffer)
        {
            if (buffer == null)
            {
                throw new NullReferenceException("buffer");
            }

            Buffer = buffer.Slice();
            SetWriterIndex(Capacity);
        }
        public ByteBufferBackedChannelBuffer(ByteBufferBackedChannelBuffer buffer)
        {
            Buffer = buffer.Buffer;
            SetIndex(buffer.ReaderIndex, buffer.WriterIndex);
        }

        public override byte[] Array => Buffer.Array;

        public override bool HasArray => Buffer.HasArray;

        public override int ArrayOffset => Buffer.ArrayOffset;

        public override void SetBytes(int index, IChannelBuffer src, int srcIndex, int length)
        {
            if (src is ByteBufferBackedChannelBuffer) {
                ByteBufferBackedChannelBuffer bbsrc = (ByteBufferBackedChannelBuffer)src;
                IByteBuffer data = bbsrc.Buffer.Duplicate();

                data.Slice(srcIndex, length);
                SetBytes(index, data);
            } else if (Buffer.HasArray)
            {
                src.GetBytes(srcIndex, Buffer.Array, index + Buffer.ArrayOffset, length);
            }
            else
            {
                src.GetBytes(srcIndex, this, index, length);
            }
        }

        public override int SetBytes(int index, MemoryStream src, int length)
        {
            int readBytes = 0;

            if (Buffer.HasArray)
            {
                index += Buffer.ArrayOffset;
                do
                {
                    int localReadBytes = src.Read(Buffer.Array, index, length);
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
            }
            else
            {
                byte[] tmp = new byte[length];
                int i = 0;
                do
                {
                    int localReadBytes = src.Read(tmp, i, tmp.Length - i);
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
                    i += readBytes;
                } while (i < tmp.Length);

                Buffer.Duplicate().SetWriterIndex(index).WriteBytes(tmp);
            }

            return readBytes;
        }

        public override int Capacity => Buffer.Capacity;

        public override IChannelBuffer Copy(int index, int len)
        {
            var dst = Buffer.Copy(index, len);
            return new ByteBufferBackedChannelBuffer(dst);
        }

        public override IChannelBufferFactory Factory => HeapChannelBufferFactory.GetInstance();

        public override byte GetByte(int index)
        {
            return Buffer.GetByte(index);
        }

        public override void GetBytes(int index, byte[] dst, int dstIndex, int length)
        {
            IByteBuffer data = Buffer.Duplicate();
            data.GetBytes(index, dst, dstIndex, length);
        }

        public override void GetBytes(int index, IByteBuffer dst)
        {
            IByteBuffer data = Buffer.Duplicate();
            dst.WriteBytes(data);
        }

        public override IByteBuffer ToByteBuffer(int index, int length)
        {
            if (index == 0 && length == Capacity)
            {
                return Buffer.Duplicate();
            }
            return Buffer.Duplicate().Slice(index, length);
        }

     

        public override void WiteBytes(byte[] src)
        {
            Buffer.WriteBytes(src);

        }


        public override void GetBytes(int index, IChannelBuffer dst, int dstIndex, int length)
        {
            var data = new byte[length];
            Buffer.GetBytes(index, data);
            dst.SetBytes(dstIndex, data, 0,length);
        }

        public override void GetBytes(int index, MemoryStream dst, int length)
        {
            throw new NotImplementedException();
        }

        public override void SetByte(int index, int value)
        {
            throw new NotImplementedException();
        }

        public override void SetBytes(int index, byte[] src, int srcIndex, int length)
        {
            throw new NotImplementedException();
        }

        public override void SetBytes(int index, IByteBuffer src)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirect => false;
    }
}

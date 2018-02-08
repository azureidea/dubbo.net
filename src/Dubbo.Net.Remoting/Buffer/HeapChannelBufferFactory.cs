using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Remoting.Buffer
{
    public class HeapChannelBufferFactory:IChannelBufferFactory
    {
        private static readonly HeapChannelBufferFactory Instance = new HeapChannelBufferFactory();

        

        public static IChannelBufferFactory GetInstance()
        {
            return Instance;
        }

        public IChannelBuffer GetBuffer(int capacity)
        {
            return ChannelBuffers.Buffer(capacity);
        }

        public IChannelBuffer GetBuffer(byte[] array, int offset, int length)
        {
            return ChannelBuffers.WrappedBuffer(array, offset, length);
        }

        public IChannelBuffer GetBuffer(IByteBuffer nioBuffer)
        {
            if (nioBuffer.HasArray)
            {
                return ChannelBuffers.WrappedBuffer(nioBuffer);
            }

            IChannelBuffer buf = GetBuffer((IByteBuffer) nioBuffer.Duplicate());
            int pos = nioBuffer.ReaderIndex;
            buf.WriteBytes(nioBuffer);
            nioBuffer.SetReaderIndex(pos);
            return buf;
        }
    }
}

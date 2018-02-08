using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Remoting.Buffer
{
    public interface IChannelBufferFactory
    {
        IChannelBuffer GetBuffer(int capacity);
        IChannelBuffer GetBuffer(byte[] ary, int off, int len);
        IChannelBuffer GetBuffer(IByteBuffer buffer);
    }
}

using DotNetty.Buffers;

namespace Dubbo.Net.Remoting.Buffer
{
    public interface IChannelBufferFactory
    {
        IChannelBuffer GetBuffer(int capacity);
        IChannelBuffer GetBuffer(byte[] ary, int off, int len);
        IChannelBuffer GetBuffer(IByteBuffer buffer);
    }
}

using System;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Netty
{
    public class InternalEncoder:MessageToByteEncoder<object>
    {
        private readonly URL _url;
        private readonly IChannelHandler _handler;
        private readonly ICodec _codec;

        public InternalEncoder(URL url, IChannelHandler handler, ICodec codec)
        {
            _url = url;
            _handler = handler;
            _codec = codec;
        }

        protected override void Encode(IChannelHandlerContext context, object message, IByteBuffer output)
        {
            DotNetty.Transport.Channels.IChannel ch = context.Channel;
            NettyChannel channel = NettyChannel.GetOrAddChannel(ch, _url, _handler);
            try
            {
                //Console.WriteLine("encode:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                _codec.Encode(channel, output, message);
                //Console.WriteLine("encoded:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            finally
            {
                NettyChannel.RemoveChannelIfDisconnected(ch);
            }
        }
    }
}

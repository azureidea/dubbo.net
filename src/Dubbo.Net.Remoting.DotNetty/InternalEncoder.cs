using System;
using System.Collections.Generic;
using System.Text;
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
                _codec.Encode(channel, output, message);
            }
            finally
            {
                NettyChannel.RemoveChannelIfDisconnected(ch);
            }
        }
    }
}

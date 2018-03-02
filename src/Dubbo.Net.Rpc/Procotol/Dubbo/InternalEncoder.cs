using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Dubbo.Net.Common;
using Dubbo.Net.Remoting;

namespace Dubbo.Net.Rpc.Procotol.Dubbo
{
    public class InternalClientEncoder : MessageToByteEncoder<Request>
    {
        private readonly ICodec _codec;
        public InternalClientEncoder(ICodec codec) { _codec = codec; }
        protected override void Encode(IChannelHandlerContext context, Request message, IByteBuffer output)
        {
            var channel = context.Channel;
            //_codec.Encode(channel, output, message);
        }
    }
    public class InternalServerEncoder : MessageToByteEncoder<Response>
    {
        private readonly ICodec _codec;
        public InternalServerEncoder(ICodec codec) { _codec = codec; }
        protected override void Encode(IChannelHandlerContext context, Response message, IByteBuffer output)
        {
            var channel = context.Channel;
            //_codec.EncodeResponse(channel, output, message);
        }
    }
}

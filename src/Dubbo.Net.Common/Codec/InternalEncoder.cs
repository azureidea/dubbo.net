using DotNetty.Codecs;
using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Dubbo.Net.Common.Codec
{
    public class InternalClientEncoder : MessageToByteEncoder<Request>
    {
        private readonly ICodec _codec;
        public InternalClientEncoder(ICodec codec) { _codec = codec; }
        protected override void Encode(IChannelHandlerContext context, Request message, IByteBuffer output)
        {
            var channel = context.Channel;
            _codec.EncodeRequest(channel, output, message);
        }
    }
    public class InternalServerEncoder : MessageToByteEncoder<Response>
    {
        private readonly ICodec _codec;
        public InternalServerEncoder(ICodec codec) { _codec = codec; }
        protected override void Encode(IChannelHandlerContext context, Response message, IByteBuffer output)
        {
            var channel = context.Channel;
            _codec.EncodeResponse(channel, output, message);
        }
    }
}

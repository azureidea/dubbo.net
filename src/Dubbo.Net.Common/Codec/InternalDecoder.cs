using DotNetty.Codecs;
using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Dubbo.Net.Common.Codec
{
    public class InternalClientDecoder : ByteToMessageDecoder
    {
        private readonly ICodec _codec;
        public InternalClientDecoder(ICodec codec) { _codec = codec; }
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            var channel = context.Channel;
            var result=_codec.DecodeResponse(channel, input);
            output.Add(result);
        }
    }
    public class InternalServerDecoder : ByteToMessageDecoder
    {
        private readonly ICodec _codec;
        public InternalServerDecoder(ICodec codec) { _codec = codec; }
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            var channel = context.Channel;
            var result = _codec.DecodeRequest(channel, input);
            output.Add(result);
        }
    }
}

using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common.Codec
{
    /// <summary>
    /// 编解码适配
    /// </summary>
    public class InternalCodecAdapter
    {
        readonly ICodec _codec;
        public InternalCodecAdapter(ICodec codec) { _codec = codec; }
        public IChannelHandler GetEncoder()
        {
            return new InternalClientEncoder(_codec);
        }
        public IChannelHandler GetDecoder()
        {
            return new InternalClientDecoder(_codec);
        }
        public IChannelHandler GetServerEncoder()
        {
            return new InternalServerEncoder(_codec);
        }
        public IChannelHandler GetServerDecoder()
        {
            return new InternalServerDecoder(_codec);
        }
    }
}

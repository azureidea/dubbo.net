using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Netty
{
    public class NettyCodecAdapter
    {
        private readonly DotNetty.Transport.Channels.IChannelHandler _encoder;
        private readonly DotNetty.Transport.Channels.IChannelHandler _decoder;
        private readonly ICodec _codec;
        private readonly URL _url;
        private readonly IChannelHandler _handler;

        public NettyCodecAdapter(ICodec codec, URL url, IChannelHandler handler)
        {
            _codec = codec;
            _url = url;
            _handler = handler;
            _encoder=new InternalEncoder(url,handler,codec);
            _decoder=new InternalDecoder(url,handler,codec);
        }

        public DotNetty.Transport.Channels.IChannelHandler GetDecoder()
        {
            return _decoder;
        }

        public DotNetty.Transport.Channels.IChannelHandler GetEncoder()
        {
            return _encoder;
        }
    }
}

using Dubbo.Net.Remoting;
using IChannelHandler = DotNetty.Transport.Channels.IChannelHandler;

namespace Dubbo.Net.Rpc.Procotol.Dubbo
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
            return null;
        }
        public IChannelHandler GetServerEncoder()
        {
            return new InternalServerEncoder(_codec);
        }
        public IChannelHandler GetServerDecoder()
        {
            return  null;
        }
    }
}

using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Remoting.Netty
{
    [DependencyIoc(typeof(ITransporter),"netty")]
    public class NettyTransporter:ITransporter
    {
        public IServer Bind(URL url, IChannelHandler handler)
        {
            return null;
        }

        public IClient Connected(URL url, IChannelHandler handler)
        {
            return new NettyClient(url,handler);
        }
    }
}

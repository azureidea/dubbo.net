using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Transport.Dispatcher.All
{
    public class AllChannelHandler:WrappedChannelHandler
    {
        public AllChannelHandler(IChannelHandler handler, URL url) : base(handler, url)
        {
        }

    }
}

using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Remoting.Transport.Dispatcher.All
{
    [DependencyIoc(typeof(IDispatcher))]
    public class AllDispatcher:IDispatcher
    {
        public IChannelHandler Dispatch(IChannelHandler handler, URL url)
        {
            return new AllChannelHandler(handler, url);
        }
    }
}

using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting
{
    public interface IDispatcher
    {
        IChannelHandler Dispatch(IChannelHandler handler, URL url);
    }
}

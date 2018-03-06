using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting
{
    public interface ITransporter
    {
        IServer Bind(URL url, IChannelHandler handler);
        IClient Connected(URL url, IChannelHandler handler);
    }
}

using System;
using System.Threading.Tasks;

namespace Dubbo.Net.Remoting
{
    public interface IChannelHandler
    {
        Task ConnectAsync(IChannel channel);
        Task DisconnectAsync(IChannel channel);
        Task SentAsync(IChannel channel, object message);
        Task RecivedAsync(IChannel channel, object message);
        Task CaughtAsync(IChannel channel, Exception exception);
    }
}

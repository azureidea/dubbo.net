using System;
using System.Threading.Tasks;

namespace Dubbo.Net.Remoting.Transport
{
    public class ChannelHandlerAdapter : IChannelHandler
    {

        public virtual async Task ConnectAsync(IChannel channel)
        {
        }

        public virtual async Task DisconnectAsync(IChannel channel)
        {
        }

        public virtual async Task SentAsync(IChannel channel, object message)
        {
        }

        public virtual async Task RecivedAsync(IChannel channel, object message)
        {
           
        }

        public virtual async Task CaughtAsync(IChannel channel, Exception exception)
        {
        }
    }
}

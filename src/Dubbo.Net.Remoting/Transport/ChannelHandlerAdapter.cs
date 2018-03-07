using System;
using System.Threading.Tasks;

namespace Dubbo.Net.Remoting.Transport
{
    public class ChannelHandlerAdapter : IChannelHandler
    {

        public virtual  Task ConnectAsync(IChannel channel)
        {
            return Task.CompletedTask;
        }

        public virtual  Task DisconnectAsync(IChannel channel)
        {
            return Task.CompletedTask;
        }

        public virtual  Task SentAsync(IChannel channel, object message)
        {
            return Task.CompletedTask;
        }

        public virtual  Task RecivedAsync(IChannel channel, object message)
        {
            return Task.CompletedTask;
        }

        public virtual  Task CaughtAsync(IChannel channel, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}

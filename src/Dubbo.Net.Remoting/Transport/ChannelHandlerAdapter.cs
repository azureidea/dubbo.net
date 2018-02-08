using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dubbo.Net.Remoting.Transport
{
    public class ChannelHandlerAdapter : IChannelHandler
    {
        public Task ConnectAsync(IChannel channel)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectAsync(IChannel channel)
        {
            throw new NotImplementedException();
        }

        public Task SentAsync(IChannel channel, object message)
        {
            throw new NotImplementedException();
        }

        public Task RecivedAsync(IChannel channel, object message)
        {
            throw new NotImplementedException();
        }

        public Task CaughtAsync(IChannel channel, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}

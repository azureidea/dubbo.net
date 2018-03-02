using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Transport.Dispatcher
{
    public class WrappedChannelHandler:IChannelHandlerDelegate
    {
        protected readonly IChannelHandler _handler;
        protected readonly URL _url;
        public WrappedChannelHandler(IChannelHandler handler, URL url)
        {
            _handler = handler;
            _url = url;
        }
        public virtual Task ConnectAsync(IChannel channel)
        {
            return _handler.ConnectAsync(channel);
        }

        public virtual Task DisconnectAsync(IChannel channel)
        {
            return _handler.DisconnectAsync(channel);
        }

        public virtual Task SentAsync(IChannel channel, object message)
        {
            return _handler.SentAsync(channel, message);
        }

        public virtual Task RecivedAsync(IChannel channel, object message)
        {
            return _handler.RecivedAsync(channel, message);
        }

        public virtual Task CaughtAsync(IChannel channel, Exception exception)
        {
            return _handler.CaughtAsync(channel, exception);
        }

        public IChannelHandler GetHandler()
        {
            if (_handler is IChannelHandlerDelegate) {
                return ((IChannelHandlerDelegate)_handler).GetHandler();
            } else {
                return _handler;
            }
        }
    }
}

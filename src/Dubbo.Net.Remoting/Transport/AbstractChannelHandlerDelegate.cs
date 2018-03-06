using System;
using System.Threading.Tasks;

namespace Dubbo.Net.Remoting.Transport
{
    public abstract class AbstractChannelHandlerDelegate:IChannelHandlerDelegate
    {
        protected IChannelHandler _handler;

        protected AbstractChannelHandlerDelegate(IChannelHandler handler)
        {
            _handler = handler;
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
            }
            return _handler;
        }
    }
}

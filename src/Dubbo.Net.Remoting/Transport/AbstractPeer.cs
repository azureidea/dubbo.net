using System;
using System.Net;
using System.Threading.Tasks;
using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Transport
{
    public abstract class AbstractPeer:IEndpoint,IChannelHandler
    {

        public AbstractPeer(URL url, IChannelHandler handler)
        {
            Url = url ?? throw new ArgumentException("url==null");
            ChannelHander = handler ?? throw new ArgumentException("handler == null");
        }

        public URL Url { get; set; }
        public IChannelHandler ChannelHander { get; set; }
        public EndPoint Address { get; set; }
        public virtual Task<Response> SendAsync(object message)
        {
            return SendAsync(message, Url.GetParameter(Constants.SentKey, false));
        }

        public abstract Task<Response> SendAsync(object message, bool sent);

        public virtual Task CloseAsync()
        {
            return Task.Factory.StartNew(() => IsClosed = true);
        }

        public virtual Task CloseAsync(int timeout)
        {
            return CloseAsync();
        }

        public bool IsClosed { get; set; }
        public virtual  async  Task ConnectAsync(IChannel channel)
        {
            if (IsClosed)
                return;
            await ChannelHander.ConnectAsync(channel);
        }

        public virtual Task DisconnectAsync(IChannel channel)
        {
            return ChannelHander.DisconnectAsync(channel);
        }

        public virtual async Task SentAsync(IChannel channel, object message)
        {
            if (IsClosed)
                throw new Exception("channel closed");
             await ChannelHander.SentAsync(channel, message);
        }

        public virtual async Task RecivedAsync(IChannel channel, object message)
        {
            if (IsClosed)
                return;
            await ChannelHander.RecivedAsync(channel, message);
        }

        public virtual Task CaughtAsync(IChannel channel, Exception exception)
        {
            return ChannelHander.CaughtAsync(channel,exception);
        }
    }
}

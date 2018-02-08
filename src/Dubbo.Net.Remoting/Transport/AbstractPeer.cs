using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Transport
{
    public abstract class AbstractPeer:IEndpoint,IChannelHandler
    {
        private bool _closed;

        public AbstractPeer(URL url, IChannelHandler handler)
        {
            Url = url ?? throw new ArgumentException("url==null");
            ChannelHander = handler ?? throw new ArgumentException("handler == null");
        }

        public URL Url { get; set; }
        public IChannelHandler ChannelHander { get; set; }
        public EndPoint Address { get; set; }
        public Task SendAsync(object message)
        {
            return SendAsync(message, Url.GetParameter(Constants.SentKey, false));
        }

        public abstract Task SendAsync(object message, bool sent);

        public Task CloseAsync()
        {
            return Task.Factory.StartNew(() => _closed = true);
        }

        public Task CloseAsync(int timeout)
        {
            return CloseAsync();
        }

        public bool IsClosed { get; set; }
        public  async  Task ConnectAsync(IChannel channel)
        {
            if (_closed)
                return;
            await ChannelHander.ConnectAsync(channel);
        }

        public Task DisconnectAsync(IChannel channel)
        {
            return ChannelHander.DisconnectAsync(channel);
        }

        public async Task SentAsync(IChannel channel, object message)
        {
            if (_closed)
                return;
            await ChannelHander.SentAsync(channel, message);
        }

        public async Task RecivedAsync(IChannel channel, object message)
        {
            if (_closed)
                return;
            await ChannelHander.RecivedAsync(channel, message);
        }

        public Task CaughtAsync(IChannel channel, Exception exception)
        {
            return ChannelHander.CaughtAsync(channel,exception);
        }
    }
}

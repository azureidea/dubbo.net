using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Remoting.Exchange.Support.Header
{
    internal class HeaderExchangeChannel : IExchangeChannel
    {
        private readonly ILogger _logger = ObjectFactory.GetInstance<ILogger>();
        public URL Url => _channel.Url;
        public IChannelHandler ChannelHander => _channel.ChannelHander;
        public EndPoint Address => _channel.Address;
        private readonly IChannel _channel;
        private volatile bool closed = false;
        private const string ChannelKey = "HeaderExchangeChannel.CHANNEL";

        internal HeaderExchangeChannel(IChannel channel)
        {
            _channel = channel ?? throw new ArgumentException("channel==null");
        }
        internal static void RemoveChannelIfDisconnected(IChannel ch)
        {
            if (ch != null && !ch.IsConnected)
            {
                ch.RemoveAttribute(ChannelKey);
            }
        }
        public Task<Response> SendAsync(object message)
        {
            return SendAsync(message, Url.GetParameter(Constants.SentKey, false));
        }

        public Task<Response> SendAsync(object message, bool sent)
        {
            if (closed)
            {
                throw new Exception("Failed to send message " + message + ", cause: The channel " + this + " is closed!");
            }

            if (message is Request || message is Response || message is string)
            {
                return _channel.SendAsync(message, sent);
            }
            Request request = new Request();
            request.Mversion = ("2.0.0");
            request.IsTwoWay = (false);
            request.Mdata = (message);
            return _channel.SendAsync(request, sent);
        }

        public async Task CloseAsync()
        {
            try
            {
                await _channel.CloseAsync();
            }
            catch (Exception e)
            {
                _logger.Warn(e);
            }
        }

        public async Task CloseAsync(int timeout)
        {
            if(closed)
                return;
            closed = true;
            if (timeout > 0)
            {
                CloseAsync().Wait(timeout);
            }
            else
            {
                await CloseAsync();
            }
        }

        public bool IsClosed => _channel.IsClosed;
        public EndPoint RemoteAddress => _channel.RemoteAddress;
        public bool IsConnected => _channel.IsConnected;
        public bool HasAttribute(string key)
        {
            return _channel.HasAttribute(key);
        }

        public object GetAttribute(string key)
        {
            return _channel.GetAttribute(key);
        }

        public void SetAttribute(string key, object value)
        {
            _channel.SetAttribute(key,value);
        }

        public void RemoveAttribute(string key)
        {
            _channel.RemoveAttribute(key);
        }

        public Task<Response> Request(object request)
        {
            return Request(request, _channel.Url.GetPositiveParameter(Constants.TimeoutKey, Constants.DefaultTimeout));
        }

        public async Task<Response> Request(object request, int timeout)
        {
            if(closed)
                throw new RemotingException(this.Address, null, "Failed to send request " + request + ", cause: The channel " + this + " is closed!");
            // create request.
            Request req = new Request();
            req.Mversion=("2.0.0");
            req.IsTwoWay=(true);
            req.Mdata=(request);
            try
            {
                return await _channel.SendAsync(req);
            }
            catch (RemotingException e)
            {
                throw e;
            }
        }

        public IExchangeHandler GetExchangeHandler()
        {
            return (IExchangeHandler)_channel.ChannelHander;
        }

        public override string ToString()
        {
            return _channel.ToString();
        }
    }
}


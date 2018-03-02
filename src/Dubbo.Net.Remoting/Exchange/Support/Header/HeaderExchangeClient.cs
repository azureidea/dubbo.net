using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Exchange.Support.Header
{
    public class HeaderExchangeClient:IExchangeClient
    {
        public URL Url => _client.Url;
        public IChannelHandler ChannelHander => _client.ChannelHander;
        public EndPoint Address => _client.Address;

        private readonly IClient _client;
        private readonly IExchangeChannel _channel;
        private int _heartbeat;
        private int _heartbeatTimeout;

        public HeaderExchangeClient(IClient client, bool needHeartBeat)
        {
            _client = client ?? throw new ArgumentException("client==null");
            _channel = new HeaderExchangeChannel(client);
            var dubbo = client.Url.GetParameter(Constants.DubboVersionKey, "2.6.0");
            _heartbeat = _client.Url.GetParameter(Constants.HeartbeatKey,
                dubbo != null && dubbo.StartsWith("1.0.") ? Constants.DefaultHeartbeat : 0);
            _heartbeatTimeout = client.Url.GetParameter(Constants.HeartbeatTimeoutKey, _heartbeat * 3);

            if (_heartbeatTimeout < _heartbeat * 2)
            {
                throw new Exception("heartbeatTimeout < heartbeatInterval * 2");
            }

            if (needHeartBeat)
            {
                StartHeartbeatTimer();
            }
        }
        public Task<Response> SendAsync(object message)
        {
            return _channel.SendAsync(message);
        }

        public Task<Response> SendAsync(object message, bool sent)
        {
            return _channel.SendAsync(message, sent);
        }

        public Task CloseAsync()
        {
            DoClose();
            return _channel.CloseAsync();
        }

        public Task CloseAsync(int timeout)
        {
            DoClose();
            return _channel.CloseAsync(timeout);
        }

        public bool IsClosed => _client.IsClosed;
        public EndPoint RemoteAddress => _client.RemoteAddress;
        public bool IsConnected => _client.IsConnected;
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

        public void Reset(URL url)
        {
            _client.Reset(url);
        }

        public Task ReconnectAsync()
        {
            return _client.ReconnectAsync();
        }

        public Task<Response> Request(object request)
        {
            return _channel.Request(request);
        }

        public Task<Response> Request(object request, int timeout)
        {
            return _channel.Request(request, timeout);
        }

        public IExchangeHandler GetExchangeHandler()
        {
            return _channel.GetExchangeHandler();
        }
        public override string ToString()
        {
            return "HeaderExchangeClient [channel=" + _channel + "]";
        }
        private void DoClose()
        {
            StopHeartbeatTimer();
        }

        private void StopHeartbeatTimer()
        {
            //todo stop heartbeat
        }

        public void StartHeartbeatTimer()
        {
            //todo start heartbeat
        }
    }
}

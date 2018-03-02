using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Remoting;
using Dubbo.Net.Remoting.Exchange;

namespace Dubbo.Net.Rpc.Procotol
{
    public sealed class LazyConnectExchangeClient : IExchangeClient
    {
        public const string RequestWithWarningKey = "lazyclient_request_with_warning";
        private readonly ILogger _logger = ObjectFactory.GetInstance<ILogger>();
        private readonly bool _requestWithWarning;
        private readonly IExchangeHandler _requestHandler;
        private readonly bool _initialState;
        private volatile IExchangeClient _client;
        private int _warningCount = 0;
        private readonly object _connectLock = new object();

        public LazyConnectExchangeClient(URL url, IExchangeHandler handler)
        {
            Url = url.AddParameter(Constants.SendReconnectKey, true);
            _requestHandler = handler;
            _initialState = url.GetParameter(Constants.LazyConnectInitialStateKey,
                Constants.DefaultLazyConnectInitialState);
            _requestWithWarning = url.GetParameter(RequestWithWarningKey, false);
        }

        public URL Url { get; set; }
        public IChannelHandler ChannelHander { get; set; }
        public EndPoint Address { get; set; }

        private void InitClient()
        {
            if (_client != null)
                return;
            if (_logger.InfoEnabled)
            {
                _logger.Info("Lazy connect to " + Url);
            }

            lock (_connectLock)
            {
                if (_client != null)
                {
                    return;
                }
                _client = Exchangers.ConnectAsync(Url, _requestHandler).Result;
            }
        }

        private void Warning(object request)
        {
            if (_requestWithWarning)
            {
                if (Interlocked.Increment(ref _warningCount) % 5000 == 0)
                {
                    _logger.Warn("safe guard client , should not be called ,must have a bug.");
                }
            }
        }

        public Task<Response> SendAsync(object message)
        {
            InitClient();
            return _client.SendAsync(message);
        }

        public Task<Response> SendAsync(object message, bool sent)
        {
            InitClient();
            return _client.SendAsync(message,sent);
        }

        public Task CloseAsync()
        {
            return _client?.CloseAsync();
        }

        public Task CloseAsync(int timeout)
        {
            return _client?.CloseAsync(timeout);
        }

        public bool IsClosed
        {
            get
            {
                if (_client != null)
                    return _client.IsClosed;
                return true;
            }
            set
            {
                
            }
        }

        public EndPoint RemoteAddress
        {
            get
            {
                if (_client == null)
                {
                    return new DnsEndPoint(Url.Ip,Url.Port);
                }

                return _client.RemoteAddress;
            }
        }

        public bool IsConnected {
            get
            {
                if (_client == null)
                    return _initialState;
                return _client.IsConnected;
            }
        }
        public bool HasAttribute(string key)
        {
            if (_client == null)
                return false;
            return _client.HasAttribute(key);
        }

        public object GetAttribute(string key)
        {
            if (_client == null)
                return false;
            return _client.GetAttribute(key);
        }

        public void SetAttribute(string key, object value)
        {
            CheckClient();
            _client.SetAttribute(key, value);
        }

        public void RemoveAttribute(string key)
        {
            CheckClient();
            _client.RemoveAttribute(key);
        }

        public void Reset(URL url)
        {
           CheckClient();
            _client.Reset(url);
        }

        public Task ReconnectAsync()
        {
            CheckClient();
            return _client.ReconnectAsync();
        }

        public Task<Response> Request(object request)
        {
            Warning(request);
            InitClient();
            return _client.Request(request);
        }

        public Task<Response> Request(object request, int timeout)
        {
            Warning(request);
            InitClient();
            return _client.Request(request,timeout);
        }

        public IExchangeHandler GetExchangeHandler()
        {
            return _requestHandler;
        }

        

        private void CheckClient()
        {
            if (_client == null)
            {
                throw new Exception(
                    "LazyConnectExchangeClient state error. the client has not be init .url:" + Url);
            }
        }
    }
}

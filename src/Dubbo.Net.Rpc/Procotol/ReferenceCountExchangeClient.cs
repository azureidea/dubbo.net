using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Remoting;
using Dubbo.Net.Remoting.Exchange;

namespace Dubbo.Net.Rpc.Procotol
{
    public sealed class ReferenceCountExchangeClient:IExchangeClient
    {
        public URL Url { get; set; }
        public IChannelHandler ChannelHander { get; set; }
        public EndPoint Address { get; set; }
        private int _referenceCount = 0;
        private IExchangeClient _client;
        private readonly ConcurrentDictionary<string, LazyConnectExchangeClient> _ghostClientMap;

        public ReferenceCountExchangeClient(IExchangeClient client,
            ConcurrentDictionary<string, LazyConnectExchangeClient> ghostClientMap)
        {
            _client = client;
            _ghostClientMap = ghostClientMap;
            Url = client.Url;
            ChannelHander = _client.ChannelHander;
            IsClosed = _client.IsClosed;
            Interlocked.Increment(ref _referenceCount);
        }


        public Task<Response> SendAsync(object message)
        {
            return _client.SendAsync(message);
        }

        public Task<Response> SendAsync(object message, bool sent)
        {
            return _client.SendAsync(message, sent);
        }

        public Task CloseAsync()
        {
            return CloseAsync(0);
        }

        public async Task CloseAsync(int timeout)
        {
            if (Interlocked.Decrement(ref _referenceCount) <= 0)
            {
                if (timeout == 0)
                {
                    await _client.CloseAsync();
                }
                else
                {
                    await _client.CloseAsync(timeout);
                }
                _client = ReplaceWithLazyClient();
            }
        }

        public bool IsClosed { get; set; }
        public EndPoint RemoteAddress => _client.RemoteAddress;
        public bool IsConnected => _client.IsConnected;
        public bool HasAttribute(string key)
        {
            return _client.HasAttribute(key);
        }

        public object GetAttribute(string key)
        {
            return _client.GetAttribute(key);
        }

        public void SetAttribute(string key, object value)
        {
            _client.SetAttribute(key,value);
        }

        public void RemoveAttribute(string key)
        {
            _client.RemoveAttribute(key);
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
            return _client.Request(request);
        }

        public Task<Response> Request(object request, int timeout)
        {
            return _client.Request(request, timeout);
        }

        public IExchangeHandler GetExchangeHandler()
        {
            return _client.GetExchangeHandler();
        }

        private LazyConnectExchangeClient ReplaceWithLazyClient()
        {
            URL lazyUrl = Url.AddParameter(Constants.LazyConnectInitialStateKey, false)
                .AddParameter(Constants.ReconnectKey, false)
                .AddParameter(Constants.SendReconnectKey, true)
                .AddParameter("warning", true)
                .AddParameter(LazyConnectExchangeClient.RequestWithWarningKey, true)
                .AddParameter("_client_memo", "referencecounthandler.replacewithlazyclient");

            var key = Url.GetAddress();
            // in worst case there's only one ghost connection.
            LazyConnectExchangeClient gclient = null;
            if (_ghostClientMap.ContainsKey(key))
            {
                 _ghostClientMap.TryGetValue(key, out gclient);
            }
            if (gclient == null || gclient.IsClosed)
            {
                gclient = new LazyConnectExchangeClient(lazyUrl, _client.GetExchangeHandler());
                _ghostClientMap.TryAdd(key, gclient);
            }
            return gclient;
        }

        public void IncreamentAndGetCount()
        {
            Interlocked.Increment(ref _referenceCount);
        }
    }
}

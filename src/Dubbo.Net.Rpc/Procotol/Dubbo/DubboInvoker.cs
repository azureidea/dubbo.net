using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Remoting;
using Dubbo.Net.Remoting.Exchange;
using Dubbo.Net.Rpc.Infrastructure;
using Dubbo.Net.Rpc.Support;
using TimeoutException = System.TimeoutException;

namespace Dubbo.Net.Rpc.Procotol.Dubbo
{
    public class DubboInvoker : AbstractInvoker
    {
        private readonly IExchangeClient[] _clients;
        private  int _index;
        private readonly string _version;
        readonly object _destroyLock = new object();
        private readonly List<IInvoker> _invokers;
        private readonly ILogger _logger = ObjectFactory.GetInstance<ILogger>();
        private readonly IConfigUtils _configUtils = ObjectFactory.GetInstance<IConfigUtils>();
        public DubboInvoker(Type type, URL url, IExchangeClient[] clients, List<IInvoker> invokers = null) : base(type, url, new string[] { Constants.InterfaceKey, Constants.GroupKey, Constants.TokenKey, Constants.TimeoutKey })
        {
            _clients = clients;
            this._version = url.GetParameter(Constants.VersionKey, "0.0.0");
            this._invokers = invokers;
        }



        protected  override async Task<IResult> DoInvoke(IInvocation invocation)
        {
            RpcInvocation inv = (RpcInvocation)invocation;
            var methodName = RpcUtils.GetMethodName(invocation);
            inv.SetAttachment(Constants.PathKey, GetUrl().Path);
            inv.SetAttachment(Constants.VersionKey, _version);

            IExchangeClient currentClient;
            if (_clients.Length == 1)
            {
                currentClient = _clients[0];
            }
            else
            {
                currentClient = _clients[Interlocked.Increment(ref _index) % _clients.Length];
            }
            try
            {
                //Console.WriteLine("step5:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                var isAsync = RpcUtils.IsAsync(GetUrl(), invocation);
                var isOneway = RpcUtils.IsOneway(GetUrl(), invocation);
                var timeout = GetUrl().GetMethodParameter(methodName, Constants.TimeoutKey, Constants.DefaultTimeout.ToString());
                if (isOneway)
                {
                    var isSent = GetUrl().GetMethodParameter(methodName, Constants.SentKey, false);
                    await currentClient.SendAsync(inv, isSent);
                    return new RpcResult();
                }
                else
                {
                    int.TryParse(timeout, out var time);
                    var future = await currentClient.Request(inv, time);
                    return new RpcResult(future);
                }
            }
            catch (TimeoutException e)
            {
                throw new Exception( "Invoke remote method timeout. method: " + invocation.MethodName + ", provider: " + GetUrl() + ", cause: " + e.Message, e);
            }
            catch (RemotingException e)
            {
                throw new Exception("Failed to invoke remote method: " + invocation.MethodName + ", provider: " + GetUrl() + ", cause: " + e.Message, e);
            }
        }
        public override bool IsAvailable()
        {
            if (!base.IsAvailable())
                return false;
            foreach (var client in _clients)
            {
                if (client.IsConnected && !client.HasAttribute(Constants.ChannelAttributeReadonlyKey))
                {
                    //cannot write == not Available ?
                    return true;
                }
            }
            return false;
        }

        public override void Destroy()
        {
            // in order to avoid closing a client multiple times, a counter is used in case of connection per jvm, every
            // time when client.close() is called, counter counts down once, and when counter reaches zero, client will be
            // closed.
            if (base.IsDestroyed())
            {
                return;
            }
            else
            {
                // double check to avoid dup close
                lock (_destroyLock)
                {

                    if (base.IsDestroyed())
                    {
                        return;
                    }
                    base.Destroy();
                    _invokers?.Remove(this);
                    foreach (var client in _clients)
                    {
                        try
                        {
                            client.CloseAsync(GetShutdownTimeout()).Wait();
                        }
                        catch (Exception t)
                        {
                            _logger.Warn(t.Message, t);
                        }
                    }
                }
            }
        }

        protected int GetShutdownTimeout()
        {
            int timeout = Constants.DefaultServerShutdownTimeout;
            var value = _configUtils.GetProperty(Constants.ShutdownWaitKey);

            if (string.IsNullOrEmpty(value))
            {
                value = _configUtils.GetProperty(Constants.ShutdownWaitSecondsKey);
            }
            if (!string.IsNullOrEmpty(value))
                int.TryParse(value, out timeout);
            return timeout;
        }
    }
}

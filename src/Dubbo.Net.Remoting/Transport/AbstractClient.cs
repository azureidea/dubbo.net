using Dubbo.Net.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Remoting.Exchange.Support.Header;
using Dubbo.Net.Rpc.Support;

namespace Dubbo.Net.Remoting.Transport
{
    public abstract class AbstractClient : AbstractEndpoint, IClient
    {
        protected static readonly string ClientThreadPoolName = "DubboClientHandler";

        private static readonly ILogger _logger = ObjectFactory.GetInstance<ILogger>();
        private readonly bool _sendReconnect;
        private readonly long _shutdownTimeout;
        private readonly int _reconnectWarningPeriod;
        private readonly object _connectLock = new object();
        private int _reconnectCount;
        private bool _connectError;
        private long _lastConnectedTime = 0;
        private volatile bool _checked = false;
        private readonly ConcurrentDictionary<long, TaskCompletionSource<Response>> _resultDictionary =
            new ConcurrentDictionary<long, TaskCompletionSource<Response>>();

        protected AbstractClient(URL url, IChannelHandler handler) : base(url, handler)
        {

            _sendReconnect = url.GetParameter(Constants.SendReconnectKey, true);

            _shutdownTimeout = url.GetParameter(Constants.ShutdownTimeoutKey, Constants.DefaultShutdownTimeout);

            // The default reconnection interval is 2s, 1800 means warning interval is 1 hour.
            _reconnectWarningPeriod = url.GetParameter("reconnect.waring.period", 1800);
        }

        protected static IChannelHandler WrapChannelHandler(URL url, IChannelHandler handler)
        {
            //url = ExecutorUtil.setThreadName(url, ClientThreadPoolName);
            url = url.AddParameterIfAbsent(Constants.ThreadpoolKey, Constants.DefaultClientThreadpool);
            return new MultiMessageHandler(
                new HeartbeatHandler(ObjectFactory.GetInstance<IDispatcher>().Dispatch(handler, url)));
        }

        public EndPoint GetConnectAddress()
        {
            return new DnsEndPoint(Url.Ip, Url.Port);
        }

        public EndPoint GetRemoteAddress()
        {
            IChannel channel = GetChannel();
            if (channel == null)
                return Url.ToInetSocketAddress();
            return channel.RemoteAddress;
        }

        public EndPoint GetLocalAddress()
        {
            IChannel channel = GetChannel();
            if (channel == null)
                return NetUtils.GetLocalAddress();
            return channel.RemoteAddress;
        }



        public object GetAttribute(string key)
        {
            IChannel channel = GetChannel();
            return channel?.GetAttribute(key);
        }

        public void SetAttribute(string key, object value)
        {
            IChannel channel = GetChannel();
            channel?.SetAttribute(key, value);
        }



        public void RemoveAttribute(string key)
        {
            IChannel channel = GetChannel();
            channel?.RemoveAttribute(key);
        }

        public EndPoint RemoteAddress { get; private set; }

        public virtual bool IsConnected { get; private set; }

        public bool HasAttribute(string key)
        {
            IChannel channel = GetChannel();
            if (channel == null)
                return false;
            return channel.HasAttribute(key);
        }

        public override async Task<Response> SendAsync(object message, bool sent)
        {
            //Console.WriteLine("step6:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            if (_sendReconnect && !IsConnected)
            {
                await ConnectAsync();
            }
            //Console.WriteLine("step7:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            IChannel channel = GetChannel();
            //TODO Can the value returned by getChannel() be null? need improvement.
            if (channel == null || !channel.IsConnected)
            {
                throw new RemotingException(this, "message can not send, because channel is closed . url:" + Url);
            }

            var id = (message as Request )?.Mid ?? 0;
            InvocationUtils.SetInvocation(id, (message as Request)?.Mdata);
            //Console.WriteLine("step8:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            var task = RegisterResultCallbackAsync(id);//todo user msg.id
            try
            {
                //Console.WriteLine("step9:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                await channel.SendAsync(message, sent);
               // Console.WriteLine("step10:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                return await task;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public override  Task RecivedAsync(IChannel channel, object message)
        {
            //Console.WriteLine("step11:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            if (message is List<object> list)
            {
                foreach (var o in list)
                {
                    var msg = o as Response;
                    DoRecive(msg);
                }
            }
            else if(message is Response msg)
            {
                DoRecive(msg);
            }
            //todo other options
            return Task.CompletedTask;
        }

        private void DoRecive(Response msg)
        {
            var id = msg?.Mid ?? 0;
            //if (_logger.DebugEnabled)
            //    _logger.Debug($"获取到id为：{id}的响应内容。");
            if (_resultDictionary.TryGetValue(id, out var task))
            {
                task.SetResult(msg);
                //return Task.CompletedTask;
            }
        }
        /// <summary>
        /// 注册指定消息的回调任务。
        /// </summary>
        /// <param name="id">消息Id。</param>
        /// <returns>远程调用结果消息模型。</returns>
        private async Task<Response> RegisterResultCallbackAsync(long id)
        {
            //if (_logger.DebugEnabled)
            //    _logger.Debug($"准备获取Id为：{id}的响应内容。");

            var task = new TaskCompletionSource<Response>();
            _resultDictionary.TryAdd(id, task);
            try
            {
                var result = await task.Task;
                return result;
            }
            finally
            {
                //删除回调任务
                _resultDictionary.TryRemove(id, out var value);
            }
        }

        public async Task ConnectAsync()
        {
            try
            {
                await DoOpenAsync();
            }
            catch (Exception t)
            {
                await CloseAsync();
                throw new RemotingException(Url.ToInetSocketAddress(), null,
                    "Failed to start " + GetType().FullName + " " + NetUtils.GetLocalAddress()
                    + " connect to the server " + GetRemoteAddress() + ", cause: " + t.Message, t);
            }
            try
            {
                // connect.
                //await ConnectAsync();
                if (Logger.InfoEnabled)
                {
                    Logger.Info("Start " + GetType().Name + " " + NetUtils.GetLocalAddress() + " connect to the server " + GetRemoteAddress());
                }
            }
            catch (RemotingException t)
            {
                if (Url.GetParameter(Constants.CheckKey, true))
                {
                    await CloseAsync();
                    throw;
                }
                else
                {
                    Logger.Warn("Failed to start " + GetType().Name + " " + NetUtils.GetLocalAddress()
                                + " connect to the server " + GetRemoteAddress() + " (check == false, ignore and retry later!), cause: " + t.Message, t);
                }
            }
            catch (Exception t)
            {
                await CloseAsync();
                throw new RemotingException(Url.ToInetSocketAddress(), null,
                    "Failed to start " + GetType().Name + " " + NetUtils.GetLocalAddress()
                    + " connect to the server " + GetRemoteAddress() + ", cause: " + t.Message, t);
            }
        }
        private async Task InitConnectStatusCheckCommand()
        {
            //reconnect=false to close reconnect
            int reconnect = GetReconnectParam(Url);
            if (reconnect > 0)
            {
                try
                {
                    if (!IsConnected)
                    {
                        await ConnectAsync();
                    }
                    else
                    {
                        _lastConnectedTime = DateTime.Now.ToTimestamp();
                    }
                }
                catch (Exception t)
                {
                    var errorMsg = "client reconnect to " + Url.Ip + " find error . url: " + Url;
                    // wait registry sync provider list
                    if (DateTime.Now.ToTimestamp() - _lastConnectedTime > _shutdownTimeout)
                    {
                        if (!_connectError)
                        {
                            _connectError = true;
                            Logger.Error(errorMsg, t);
                            return;
                        }
                    }

                    if (Interlocked.Increment(ref _reconnectCount) % _reconnectWarningPeriod == 0)
                    {
                        Logger.Warn(errorMsg, t);
                    }
                }
            };
        }
        public async Task DisconnectAsync()
        {
            //todo 停止心跳监测
            try
            {
                IChannel channel = GetChannel();
                channel?.CloseAsync().Wait();
            }
            catch (Exception e)
            {
                Logger.Warn(e.Message, e);
            }

            try
            {
                await DoDisConnectAsync();
            }
            catch (Exception e)
            {
                Logger.Warn(e.Message, e);
            }

        }

        public async Task ReconnectAsync()
        {
            await DisconnectAsync();
            await ConnectAsync();
        }

        public override async Task CloseAsync()
        {
            try
            {
                //todo停止心跳包检测
            }
            catch (Exception e)
            {
                Logger.Warn(e.Message, e);
            }
            try
            {
                await base.CloseAsync();
            }
            catch (Exception e)
            {
                Logger.Warn(e.Message, e);
            }
            try
            {
                await DisconnectAsync();
            }
            catch (Exception e)
            {
                Logger.Warn(e.Message, e);
            }
            try
            {
                await DoCloseAsync();
            }
            catch (Exception e)
            {
                Logger.Warn(e.Message, e);
            }
        }

        public override async Task CloseAsync(int timeout)
        {
            //ExecutorUtil.gracefulShutdown(executor, timeout);
            await CloseAsync();
        }

        public override string ToString()
        {
            return GetType().FullName + " [" + GetLocalAddress() + " -> " + GetRemoteAddress() + "]";
        }

        private static int GetReconnectParam(URL url)
        {
            int reconnect;
            var param = url.GetParameter(Constants.ReconnectKey, "");
            if (string.IsNullOrEmpty(param) || "true".Equals(param, StringComparison.OrdinalIgnoreCase))
            {
                reconnect = Constants.DefaultReconnectPeriod;
            }
            else if ("false".Equals(param, StringComparison.OrdinalIgnoreCase))
            {
                reconnect = 0;
            }
            else
            {
                try
                {
                    reconnect = int.Parse(param);
                }
                catch (Exception)
                {
                    throw new ArgumentException("reconnect param must be nonnegative integer or false/true. input is:" + param);
                }
                if (reconnect < 0)
                {
                    throw new ArgumentException("reconnect param must be nonnegative integer or false/true. input is:" + param);
                }
            }
            return reconnect;
        }
        /**
         * Open client.
         *
         * @throws Throwable
         */
        protected abstract Task DoOpenAsync();

        /**
         * Close client.
         *
         * @throws Throwable
         */
        protected abstract Task DoCloseAsync();

        /**
         * Connect to server.
         *
         * @throws Throwable
         */
        protected abstract Task DoConnectAsync();

        /**
         * disConnect to server.
         *
         * @throws Throwable
         */
        protected abstract Task DoDisConnectAsync();

        /**
         * Get the connected channel.
         *
         * @return channel
         */
        protected abstract IChannel GetChannel();

    }
}

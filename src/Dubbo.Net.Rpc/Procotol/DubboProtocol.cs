using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Serialize.Supports;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Remoting;
using Dubbo.Net.Remoting.Exchange;
using Dubbo.Net.Remoting.Exchange.Support;
using Dubbo.Net.Rpc.Infrastructure;
using Dubbo.Net.Rpc.Procotol.Dubbo;

namespace Dubbo.Net.Rpc.Procotol
{
    [DependencyIoc(typeof(IProtocol), "dubbo")]
    public class DubboProtocol : AbstractProtocol
    {
        private const int DefaultPort = 20880;

        private const string IsCallbackServiceInvoke = "_isCallBackServiceInvoke";
        private readonly ConcurrentDictionary<string, IExchangeServer> _servers = new ConcurrentDictionary<string, IExchangeServer>();
        private readonly ConcurrentDictionary<string, ReferenceCountExchangeClient> _referenceClients = new ConcurrentDictionary<string, ReferenceCountExchangeClient>();

        private readonly ConcurrentDictionary<string, LazyConnectExchangeClient> _ghostClients = new ConcurrentDictionary<string, LazyConnectExchangeClient>();
        private readonly object _lockObj = new object();
        private readonly List<string> _optimizers = new List<string>();
        private readonly ILogger _logger = ObjectFactory.GetInstance<ILogger>();

        private readonly ConcurrentDictionary<string, string> _stubServiceMethods = new ConcurrentDictionary<string, string>();

        
        private IExchangeHandler _requestHandler = null;

        private IExchangeHandler RequestHandler()
        {
            return _requestHandler ?? (_requestHandler = new DubboExchangeHandler(this));
        }

        public override int GetDefaultPort()
        {
            return DefaultPort;
        }

        public override IExporter Export(IInvoker invoker)
        {
            var url = invoker.GetUrl();
            var key = ServiceKey(url);
            var exporter = new DubboExporter(invoker, key, Exporters);
            Exporters.TryAdd(key, exporter);
            var isStubSupportEvent = url.GetParameter(Constants.StubEventKey, Constants.DefaultStubEvent);
            var isCallbackService = url.GetParameter(Constants.IsCallbackService, false);
            if (isStubSupportEvent && !isCallbackService)
            {
                var stubServiceMethods = url.GetParameter(Constants.StubEventMethodsKey, "");
                if (string.IsNullOrEmpty(stubServiceMethods))
                {
                    if (Logger.WarnEnabled)
                    {
                        Logger.Warn(new Exception("consumer [" + url.GetParameter(Constants.InterfaceKey, "") +
                                                              "], has set stubproxy support event ,but no stub methods founded."));
                    }
                }
                else
                {
                    _stubServiceMethods.TryAdd(url.ServiceName, stubServiceMethods);
                }
            }
            OpenServer(url);
            OptimizeSerialization(url);
            return exporter;
        }
        private void OpenServer(URL url)
        {
            // find server.
            var key = url.GetAddress();
            //client can export a service which's only for server to invoke
            var isServer = url.GetParameter(Constants.IsServerKey, true);
            if (isServer)
            {
                _servers.TryGetValue(key, out var server);
                if (server == null)
                {
                    _servers.TryAdd(key, CreateServer(url));
                }
                else
                {
                    // server supports reset, use together with override
                    server.Reset(url);
                }
            }
        }

        private IExchangeServer CreateServer(URL url)
        {
            // send readonly event when server closes, it's enabled by default
            url = url.AddParameterIfAbsent(Constants.ChannelReadonlyeventSentKey, "true");
            // enable heartbeat by default
            url = url.AddParameterIfAbsent(Constants.HeartbeatKey, Constants.DefaultHeartbeat.ToString());
            var str = url.GetParameter(Constants.ServerKey, Constants.DefaultRemotingServer);
            ITransporter transporter = null;
            if (!string.IsNullOrEmpty(str))
            {
                transporter = ObjectFactory.GetInstance<ITransporter>(str);
            }
            if (transporter == null)
                throw new Exception("Unsupported server type: " + str + ", url: " + url);

            url = url.AddParameter(Constants.CodecKey, DubboCodec.Name);
            IExchangeServer server;
            try
            {
                server = Exchangers.BindAsync(url, RequestHandler()).Result;
            }
            catch (RemotingException e)
            {
                throw new Exception("Fail to start server(url: " + url + ") " + e.Message);
            }
            str = url.GetParameter(Constants.ClientKey, "");
            if (!string.IsNullOrEmpty(str))
            {
                var supportedTypes = ObjectFactory.GetTypeKeys<ITransporter>();
                if (!supportedTypes.Contains(str))
                {
                    throw new Exception("Unsupported client type: " + str);
                }
            }
            return server;
        }

        private void OptimizeSerialization(URL url)
        {
            String className = url.GetParameter(Constants.OptimizerKey, "");
            if (string.IsNullOrEmpty(className) || _optimizers.Contains(className))
            {
                return;
            }
            Logger.Info("Optimizing the serialization process for Kryo, FST, etc...");
            var clazz = TypeMatch.MatchType(className);
            if (!typeof(ISerializationOptimizer).IsAssignableFrom(clazz))
            {
                throw new Exception("The serialization optimizer " + className + " isn't an instance of " + typeof(ISerializationOptimizer).FullName);
            }
            ISerializationOptimizer optimizer = (ISerializationOptimizer)Activator.CreateInstance(clazz);
            if (optimizer.GetSerializableClasses() == null)
            {
                return;
            }
            foreach (var c in optimizer.GetSerializableClasses())
            {
                SerializableClassRegistry.RegisterClass(c);
            }
            _optimizers.Add(className);
        }



        public override IInvoker Refer(URL url)
        {
            OptimizeSerialization(url);
            var id = url.GetId();
            var name = url.ServiceName;
            IInvoker invoker = null;
            lock (_invokers)
            {
                if (_invokers.TryGetValue(name, out var list))
                {
                    invoker = list.FirstOrDefault(c => c.InvokerId == id);
                    if (invoker != null)
                        return invoker;
                }
                else
                {
                    list = new List<IInvoker>();
                    _invokers.TryAdd(name, list);
                }
                var type = TypeMatch.MatchType(name);
                invoker = new DubboInvoker(type, url, GetClients(url), null);
                invoker.InvokerId = id;
                list.Add(invoker);
            }
            return invoker;
        }

        public override void Destroy()
        {
            foreach (var server in _servers.Values)
            {
                if (server != null)
                {
                    try
                    {
                        if (_logger.InfoEnabled)
                        {
                            _logger.Info("Close dubbo server: " + server.Address);
                        }
                        server.CloseAsync(GetServerShutdownTimeout()).Wait();
                    }
                    catch (Exception t)
                    {
                        _logger.Warn(t.Message, t);
                    }
                }
            }

            foreach (var client in _referenceClients.Values)
            {
                if (client != null)
                {
                    try
                    {
                        if (_logger.InfoEnabled)
                        {
                            _logger.Info("Close dubbo connect: " + client.Address + "-->" + client.RemoteAddress);
                        }
                        client.CloseAsync(GetServerShutdownTimeout()).Wait();
                    }
                    catch (Exception t)
                    {
                        _logger.Warn(t.Message, t);
                    }
                }
            }

            foreach (var client in _ghostClients.Values)
            {
                if (client != null)
                {
                    try
                    {
                        if (_logger.InfoEnabled)
                        {
                            _logger.Info("Close dubbo connect: " + client.Address + "-->" + client.RemoteAddress);
                        }
                        client.CloseAsync(GetServerShutdownTimeout()).Wait();
                    }
                    catch (Exception t)
                    {
                        _logger.Warn(t);
                    }
                }
            }
            _stubServiceMethods.Clear();
            base.Destroy();
        }

        private IExchangeClient[] GetClients(URL url)
        {
            // whether to share connection
            var serviceShareConnect = false;
            int connections = url.GetParameter(Constants.ConnectionsKey, 0);
            // if not configured, connection is shared, otherwise, one connection for one service
            if (connections == 0)
            {
                serviceShareConnect = true;
                connections = 1;
            }

            var clients = new IExchangeClient[connections];
            for (int i = 0; i < clients.Length; i++)
            {
                if (serviceShareConnect)
                {
                    clients[i] = GetSharedClient(url);
                }
                else
                {
                    clients[i] = InitClient(url);
                }
            }
            return clients;
        }

        private IExchangeClient GetSharedClient(URL url)
        {
            var key = url.GetAddress();
            ReferenceCountExchangeClient client;
            if (_referenceClients.ContainsKey(key))
            {
                client = _referenceClients[key];
                if (!client.IsClosed)
                {
                    client.IncreamentAndGetCount();
                    return client;
                }
                else
                {
                    _referenceClients.TryRemove(key, out var c);
                }
            }
            lock (_lockObj)
            {
                var exchangeClient = InitClient(url);
                client = new ReferenceCountExchangeClient(exchangeClient, _ghostClients);
                _referenceClients.TryAdd(key, client);
                _ghostClients.TryRemove(key, out var g);
                return client;
            }
        }
        private IExchangeClient InitClient(URL url)
        {

            // client type setting.
            var str = url.GetParameter(Constants.ClientKey, url.GetParameter(Constants.ServerKey, Constants.DefaultRemotingClient));

            var version = url.GetParameter(Constants.DubboVersionKey, "2.6.0");
            var compatible = (version != null && version.StartsWith("1.0."));
            url = url.AddParameter(Constants.CodecKey, DubboCodec.Name);
            // enable heartbeat by default
            url = url.AddParameterIfAbsent(Constants.HeartbeatKey, Constants.DefaultHeartbeat.ToString());

            // BIO is not allowed since it has severe performance issue.
            var transporter = ObjectFactory.GetInstance<ITransporter>(str);
            if (transporter == null)
            {
                throw new Exception("Unsupported client type: " + str + "," +
                                       " supported client type is " + ObjectFactory.GetTypeKeys<ITransporter>() + " ");
            }

            IExchangeClient client;
            try
            {
                // connection should be lazy
                if (url.GetParameter(Constants.LazyConnectKey, false))
                {
                    client = new LazyConnectExchangeClient(url, RequestHandler());
                }
                else
                {
                    client = Exchangers.ConnectAsync(url, RequestHandler()).Result;
                }
            }
            catch (RemotingException e)
            {
                throw new Exception("Fail to create remoting client for service(" + url + "): " + e.Message, e);
            }
            return client;
        }
        private IInvoker GetInvoker(IChannel channel, IInvocation inv)
        {
            var isCallBackServiceInvoke = false;
            var isStubServiceInvoke = false;
            var port = channel.Url.Port;//todo url.localaddress
            var path = inv.GetAttachment(Constants.PathKey);
            isStubServiceInvoke = "true".Equals(inv.GetAttachment(Constants.StubEventKey), StringComparison.OrdinalIgnoreCase);
            if (isStubServiceInvoke)
                port = channel.Url.Port;
            isCallBackServiceInvoke = IsClientSide(channel) && !isStubServiceInvoke;
            if (isCallBackServiceInvoke)
            {
                path = path + "." + inv.GetAttachment(Constants.CallbackServiceKey);
                inv.Attachments.Add(IsCallbackServiceInvoke, "true");
            }

            var serviceKey = ServiceKey(port, path, inv.GetAttachment(Constants.VersionKey),
                inv.GetAttachment(Constants.GroupKey));
            Exporters.TryGetValue(serviceKey, out var exporter);
            return exporter?.GetInvoker();
        }
        private bool IsClientSide(IChannel channel)
        {
            var address = (DnsEndPoint)channel.RemoteAddress;
            URL url = channel.Url;
            return url.Port == address.Port && url.Ip == address.Host;
        }
        class DubboExchangeHandler : ExchangeHandlerAdapter
        {
            private readonly DubboProtocol _protocol;

            public DubboExchangeHandler(DubboProtocol protocol)
            {
                _protocol = protocol;
            }

            public override async Task<object> Reply(IExchangeChannel channel, object request)
            {
                if (request is IInvocation inv)
                {
                    var invoker = _protocol.GetInvoker(channel, inv);
                    if ("true".Equals(inv.GetAttachment(IsCallbackServiceInvoke), StringComparison.OrdinalIgnoreCase))
                    {
                        var methodStr = invoker.GetUrl().GetParameter("methods", "");
                        var hasMethod = false;
                        if (methodStr == null || methodStr.IndexOf(",", StringComparison.Ordinal) == -1)
                        {
                            hasMethod = inv.MethodName == methodStr;
                        }
                        else
                        {
                            var methods = methodStr.Split(',');
                            hasMethod = methods.Contains(inv.MethodName);
                        }

                        if (!hasMethod)
                        {
                            _protocol.Logger.Warn("", new Exception("The methodName " + inv.MethodName + " not found in callback service interface ,invoke will be ignored. please update the api interface. url is:" + invoker.GetUrl() + " ,invocation is :" + inv));
                            return null;
                        }
                    }

                    return await invoker.Invoke(inv);
                }
                throw new RemotingException(channel, "Unsupported request: " + (request == null ? null : request.GetType().Name + ": " + request) + ", channel: consumer: " + channel.RemoteAddress + " --> provider: " + channel.RemoteAddress);
            }

            public override async Task RecivedAsync(IChannel channel, object message)
            {
                if (message is IInvocation)
                    await Reply((IExchangeChannel)channel, message);
                else
                    await base.RecivedAsync(channel, message);
            }

            public override Task ConnectAsync(IChannel channel)
            {
                return Invoke(channel, Constants.OnConnectKey);
            }

            public override async Task DisconnectAsync(IChannel channel)
            {
                if (_protocol.Logger.InfoEnabled)
                    _protocol.Logger.Info("disconected from " + channel.RemoteAddress + ",url:" + channel.Url);
                await Invoke(channel, Constants.OnDisconnectKey);
            }

            private async Task Invoke(IChannel channel, string methodKey)
            {
                var invocation = CreateInvocation(channel, channel.Url, methodKey);
                if (invocation != null)
                {
                    try
                    {
                        await RecivedAsync(channel, invocation);
                    }
                    catch (Exception e)
                    {
                        _protocol.Logger.Warn("Failed to invoke event method " + invocation.MethodName + "(), cause: " + e.Message, e);
                    }
                }
            }
            private IInvocation CreateInvocation(IChannel channel, URL url, string methodKey)
            {
                var method = url.GetParameter(methodKey, "");
                if (string.IsNullOrEmpty(method))
                {
                    return null;
                }
                RpcInvocation invocation = new RpcInvocation();
                invocation.SetAttachment(Constants.PathKey, url.Path);
                invocation.SetAttachment(Constants.GroupKey, url.GetParameter(Constants.GroupKey, ""));
                invocation.SetAttachment(Constants.InterfaceKey, url.GetParameter(Constants.InterfaceKey, ""));
                invocation.SetAttachment(Constants.VersionKey, url.GetParameter(Constants.VersionKey, ""));
                if (url.GetParameter(Constants.StubEventKey, false))
                {
                    invocation.SetAttachment(Constants.StubEventKey, "true");
                }
                return invocation;
            }
        }
    }
}

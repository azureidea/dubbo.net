using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Rpc.Infrastructure
{
    public abstract class AbstractProtocol:IProtocol
    {
        protected readonly ConcurrentDictionary<string,IExporter> Exporters=new ConcurrentDictionary<string, IExporter>();

        protected readonly ConcurrentDictionary<string, List<IInvoker>> _invokers = new ConcurrentDictionary<string, List<IInvoker>>();

        protected AbstractProtocol()
        {
        }
        protected readonly ILogger Logger = ObjectFactory.GetInstance<ILogger>();
        private static IConfigUtils _configUtils = ObjectFactory.GetInstance<IConfigUtils>();

        protected static string ServiceKey(URL url)
        {
            return url.ServiceName;
        }

        protected static string ServiceKey(int port, string serviceName, string serviceVersion, string serviceGroup)
        {
            return serviceName;
        }

        protected static int GetServerShutdownTimeout()
        {
            var timeout = Constants.DefaultServerShutdownTimeout;
            var value = _configUtils.GetProperty(Constants.ShutdownWaitKey);
            if (!string.IsNullOrEmpty(value))
            {
                int.TryParse(value, out timeout);
            }
            else
            {
                value = _configUtils.GetProperty(Constants.ShutdownWaitSecondsKey);
                if (!string.IsNullOrEmpty(value))
                {
                    if(int.TryParse(value, out timeout))
                    {
                        timeout *=  1000;
                    }
                }

            }

            return timeout;
        }
        public abstract int GetDefaultPort();

        public abstract IExporter Export(IInvoker invoker);

        public abstract IInvoker Refer(URL url);
        List<IInvoker> IProtocol.Invokers(string serviceName)
        {
            if (_invokers.TryGetValue(serviceName, out var list))
            {
                return list;
            }
            return new List<IInvoker>();
        }

        public virtual void Destroy()
        {
            foreach (var kv in _invokers)
            {
                foreach (var invoker in kv.Value)
                {
                    if (invoker != null)
                    {
                        kv.Value.Remove(invoker);
                        try
                        {
                            if (Logger.InfoEnabled)
                            {
                                Logger.Info("Destroy reference: " + invoker.GetUrl());
                            }
                            invoker.Destroy();
                        }
                        catch (Exception t)
                        {
                            Logger.Warn(t.Message, t);
                        }
                    }
                }
            }
            foreach (var key in Exporters.Keys)
            {
                 Exporters.TryRemove(key, out var exporter);
                if (exporter != null)
                {
                    try
                    {
                        if (Logger.InfoEnabled)
                        {
                            Logger.Info("Unexport service: " + exporter.GetInvoker().GetUrl());
                        }
                        exporter.UnExport();
                    }
                    catch (Exception t)
                    {
                        Logger.Warn(t.Message, t);
                    }
                }
            }
        }
    }
}

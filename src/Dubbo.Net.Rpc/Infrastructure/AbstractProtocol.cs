using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Rpc.Infrastructure
{
    public abstract class AbstractProtocol:IProtocol
    {
        protected readonly ConcurrentDictionary<string,IExporter> Exporters=new ConcurrentDictionary<string, IExporter>();

        readonly List<IInvoker> _invokers;

        protected AbstractProtocol()
        {
            _invokers=new List<IInvoker>();
        }
        public  List<IInvoker> Invokers=>_invokers;
        protected readonly ILogger Logger = ObjectFactory.GetInstance<ILogger>();
        private static IConfigUtils _configUtils = ObjectFactory.GetInstance<IConfigUtils>();

        protected static string ServiceKey(URL url)
        {
            return "";
        }

        protected static string ServiceKey(int port, string serviceName, string serviceVersion, string serviceGroup)
        {
            return "";
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

        public virtual void Destroy()
        {
            foreach (IInvoker invoker in Invokers)
            {
                if (invoker != null)
                {
                    Invokers.Remove(invoker);
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

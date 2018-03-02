using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Dubbo.Net.Rpc.Infrastructure;

namespace Dubbo.Net.Rpc.Procotol.Dubbo
{
    public class DubboExporter:AbstractExporter
    {
        private readonly string _key;
        private readonly ConcurrentDictionary<string, IExporter> _exporters;
        public DubboExporter(IInvoker invoker,string key,ConcurrentDictionary<string,IExporter> exporters) : base(invoker)
        {
            _key = key;
            _exporters = exporters;
        }

        public override void UnExport()
        {
            base.UnExport();
            _exporters.TryRemove(_key, out var value);
        }
    }
}


using System;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Rpc.Infrastructure
{
    public abstract class AbstractExporter:IExporter
    {
        private ILogger _logger = ObjectFactory.GetInstance<ILogger>();
        private IInvoker _Invoker;
        private volatile bool _unexported = false;

        protected AbstractExporter(IInvoker invoker)
        {
            if (invoker == null)
                throw new Exception("service invoker == null");
            if (invoker.GetInterface() == null)
                throw new Exception("service type == null");
            if (invoker.GetUrl() == null)
                throw new Exception("service url == null");
            _Invoker = invoker;
        }

        public virtual IInvoker GetInvoker()
        {
            return _Invoker;
        }

        public virtual void UnExport()
        {
            if(_unexported)
                return;
            _unexported = true;
            _Invoker.Destroy();
        }

        public override string ToString()
        {
            return _Invoker.ToString();
        }
    }
}

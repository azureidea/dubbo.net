using System.Collections.Generic;
using Dubbo.Net.Common;

namespace Dubbo.Net.Rpc.Infrastructure
{
    public interface IProtocol
    {
        int GetDefaultPort();
        IExporter Export(IInvoker invoker);
        IInvoker Refer(URL url);
        List<IInvoker> Invokers { get; }
        void Destroy();
    }
}

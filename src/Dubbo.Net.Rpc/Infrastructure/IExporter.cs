using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Rpc.Infrastructure
{
    public interface IExporter
    {
        IInvoker GetInvoker();
        void UnExport();
    }
}

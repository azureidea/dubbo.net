using System;
using System.Collections.Generic;
using System.Text;
using Dubbo.Net.Common;

namespace Dubbo.Net.Rpc.Registry
{
    public interface INotifyListener
    {
        void Notify(List<URL> urls);
    }
}

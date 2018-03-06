using System;
using System.Collections.Generic;
using System.Text;
using Dubbo.Net.Common;

namespace Dubbo.Net.Rpc.Registry
{
    public interface IRegistryFactory
    {
        IRegistry GetRegistry(URL url);
    }
}

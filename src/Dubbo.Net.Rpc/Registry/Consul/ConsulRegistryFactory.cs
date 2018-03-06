using System;
using System.Collections.Generic;
using System.Text;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Rpc.Registry.Consul
{
    [DependencyIoc(typeof(IRegistryFactory),"consul")]
    public class ConsulRegistryFactory:AbstractRegistryFactory
    {
        protected override IRegistry CreateRegistry(URL url)
        {
            return new ConsulRegistry(url);
        }
    }
}

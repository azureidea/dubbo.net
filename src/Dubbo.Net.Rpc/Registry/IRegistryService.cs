using System.Collections.Generic;
using System.Threading.Tasks;
using Dubbo.Net.Common;

namespace Dubbo.Net.Rpc.Registry
{
    public interface IRegistryService
    {
        Task Register(URL url);
        Task UnRegister(string serviceId);
        Task Subscribe(string serviceName);
        Task<List<URL>> Lookup(string serviceName);
        Task UnSubscribe(string serviceName);
    }
}

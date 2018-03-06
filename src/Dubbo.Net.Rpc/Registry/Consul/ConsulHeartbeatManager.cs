using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dubbo.Net.Rpc.Registry.Consul
{
    public class ConsulHeartbeatManager
    {
        private ConsulEcwidClient _client;
        private readonly ConcurrentDictionary<string,string> _serviceIds=new ConcurrentDictionary<string,string>();

        public ConsulHeartbeatManager(ConsulEcwidClient client)
        {
            _client = client;
        }

        public void AddServiceId(string serviceId)
        {
            _serviceIds.TryAdd(serviceId,serviceId);
        }

        public void RemoveServiceId(string serviceId)
        {
            _serviceIds.TryRemove(serviceId,out var id);
        }

        public void Start() { }

       

    }
}

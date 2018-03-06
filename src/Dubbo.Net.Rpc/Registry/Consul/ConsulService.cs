using System;
using System.Collections.Generic;
using System.Text;
using Dubbo.Net.Common;

namespace Dubbo.Net.Rpc.Registry.Consul
{
    public class ConsulService
    {
        public ConsulService()
        {

        }
        public ConsulService(URL url) { }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public long Ttl { get; set; }
    }
}

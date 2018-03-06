using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Rpc.Registry.Consul
{
    public class ConsulResponse<T> 
    {
        public T Value { get; set; }
        public ulong ConsulIndex { get; set; }
        public TimeSpan ConsulLastContact { get; set; }
        public bool ConsulKnownLeader { get; set; }
    }
}

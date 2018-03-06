using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Dubbo.Net.Common;

namespace Dubbo.Net.Rpc.Registry
{
    public abstract class AbstractRegistryFactory:IRegistryFactory
    {
        private static readonly  ConcurrentDictionary<string,IRegistry> Registers=new ConcurrentDictionary<string, IRegistry>();

        public static void DestroyAll()
        {
            foreach (var register in Registers)
            {
                register.Value.Destroy();
            }
            Registers.Clear();
        }
        public IRegistry GetRegistry(URL url)
        {
            var key = url.ToRegistryString();
            if (Registers.TryGetValue(key, out var registry))
                return registry;
            registry = CreateRegistry(url);
            Registers.TryAdd(key, registry);
            return registry;
        }

        protected abstract IRegistry CreateRegistry(URL url);
    }
}

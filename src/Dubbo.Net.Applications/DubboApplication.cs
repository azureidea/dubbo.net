using System;
using System.Collections.Generic;
using System.Reflection;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Attributes;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Config;
using Dubbo.Net.Rpc.Registry;

namespace Dubbo.Net.Applications
{
    public class DubboApplication
    {
        /// <summary>
        /// 初始化Dubbo消费端
        /// </summary>
        public static void Init(ProtocolConfig protocol, RegistryConfig registryConfig)
        {
            DependencyRegistor.Register("Dubbo.Net.Applications");
            var registryUrl = URL.ValueOf(registryConfig.ToRegistryString());
            var registryFactory = ObjectFactory.GetInstance<IRegistryFactory>(registryUrl.Protocol);
            var registry = registryFactory.GetRegistry(registryUrl);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var ts = assembly.GetTypes();
                foreach (var type in ts)
                {
                    if (!type.IsInterface)
                        continue;
                    var attr = type.GetCustomAttribute<ReferAttribute>();
                    if (attr != null)
                    {
                        types.Add(type);
                    }
                }
            }

            foreach (var type in types)
            {
                TypeMatch.RegisterType(type);
                var url = ReferenceConfig.Init(type, registryConfig, protocol);
                var impl = TypeCreator.ImplType(type);
                var ctor = impl.GetConstructor(new[] { typeof(URL) });
                var service = ctor.Invoke(new object[] { url });
                ObjectFactory.Register(type, impl);
                ObjectFactory.Register(impl, service);
                registry.Subscribe(url.ServiceName).Wait();
            }
            registry.Start();
        }
    }
}

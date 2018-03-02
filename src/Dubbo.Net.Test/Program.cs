using Dubbo.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dubbo.Net.Common.Serialize.Supports.Json;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Proxy;
using Dubbo.Net.Remoting;
using Dubbo.Net.Remoting.Exchange;
using Dubbo.Net.Remoting.Exchange.Support.Header;
using Dubbo.Net.Remoting.Netty;
using Dubbo.Net.Remoting.Transport;
using Dubbo.Net.Remoting.Transport.Dispatcher.All;
using Dubbo.Net.Rpc.Infrastructure;
using Dubbo.Net.Rpc.Procotol;

namespace Dubbo.Net.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = new URL
            {
                Ip = "192.168.2.90",
                Port = 20882,
                ServiceName = "com.mc.userconnect.api.service.PafUserService",
                Path = "com.mc.userconnect.api.service.PafUserService",
            };
            url.AddParameterIfAbsent("send.reconnect", "true");
            //ObjectFactory.Register<ILogger,ConsoleLog>();
            //ObjectFactory.Register<IDispatcher,AllDispatcher>();
            //ObjectFactory.Register<IProtocol,DubboProtocol>("dubbo");
            //ObjectFactory.Register<ITransporter,NettyTransporter>("netty");
            //ObjectFactory.Register<IExchanger,HeaderExchanger>("header");
            //ObjectFactory.Register<IConfigUtils, CacheConfigUtil>();
            //CodecSupport.RegisterSerializer(new FastJsonSerialization());
            DependencyRegistor.Register( "Dubbo.Net.Test");
            RequestTest(url);
            
            Console.ReadLine();
        }

        static void RequestTest(URL url)
        {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var ts = assembly.GetTypes();
                foreach (var type in ts)
                {
                    if(!type.IsInterface)
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
                //todo generator url from config
                var impl = TypeCreator.NewAssembly(type);
                var ctor = impl.GetConstructor(new[] {typeof(URL)});
                var service = ctor.Invoke(new object[] {url});
                ObjectFactory.Register(type,impl);
                ObjectFactory.Register(impl,service);
            }

            var s = ObjectFactory.GetInstance<IPafUserService>();


            var result1 =  s.GetAccessToken(10024, 1191382).Result;

        }


    }

   
}

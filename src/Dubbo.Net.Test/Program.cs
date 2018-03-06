using Dubbo.Net.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Dubbo.Net.Applications;
using Dubbo.Net.Common.Serialize;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Config;

namespace Dubbo.Net.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            var protocol = new ProtocolConfig
            {
                Name = "dubbo",
                Serialize = "fastjson"
            };
            var registryConfig = new RegistryConfig("consul://127.0.0.1:8500");

            DubboApplication.Init(protocol,registryConfig);
            //ObjectFactory.Register<ILogger,ConsoleLog>();
            //ObjectFactory.Register<IConfigUtils, CacheConfigUtil>();
            RequestTest(20000,true);
            
            Console.ReadLine();
        }

        static void RequestTest(int count,bool parall=false )
        {
            var s = ObjectFactory.GetInstance<IPafUserService>();
            var result1 =  s.GetAccessToken(10024, 1191382).Result;
            var i = 0;
            var watch = new Stopwatch();
            watch.Start();
            if (parall)
            {
                var tasks = new List<Task>(count);
                while (i < count)
                {
                    tasks.Add(s.GetAccessToken(10024, 1191382));
                    i++;
                }
                 Task.WaitAll(tasks.ToArray());
            }
            else
            {
                while (i < count)
                {
                    s.GetAccessToken(10024, 1191382).Wait();
                    i++;
                }
            }
            watch.Stop();
            
            Console.WriteLine($"{i}次调用，共耗时:{watch.ElapsedMilliseconds}ms,平均耗时:{Math.Round((decimal)watch.ElapsedMilliseconds/i,2)}ms");
        }


    }

   
}

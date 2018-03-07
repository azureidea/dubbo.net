using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dubbo.Net.Common.Utils
{
    public static class DependencyRegistor
    {
        /// <summary>
        /// 注册依赖注入
        /// </summary>
        /// <param name="assemblies"></param>
        public static void Register(params string[] assemblies)
        {
            var list = new List<string>
            {
                "Dubbo.Net.Common",
                "Dubbo.Net.Remoting",
                "Dubbo.Net.Remoting.Netty",
                "Dubbo.Net.Rpc",
            };
            list.AddRange(assemblies);
            //ObjectFactory.Register(typeof(IPafUserRepository),typeof(PafUserRepository));
            //ObjectFactory.Register(typeof(IPafUserService),typeof(PafUserService));
            var ass= AppDomain.CurrentDomain.GetAssemblies().ToList();
            var assemblys = list.Concat(ass.Select(a=>a.FullName)).Select(Assembly.Load).ToList(); //AppDomain.CurrentDomain.GetAssemblies().ToList();
            List<Type> types = new List<Type>();
            foreach (var assembly in assemblys)
            {
                var items = assembly.GetTypes().Where(a => a.GetCustomAttributes<DependencyIocAttribute>().Any()).ToList();
                if (items.Any())
                    types.AddRange(items);
            }
            foreach (var type in types)
            {
                var attr = type.GetCustomAttributes<DependencyIocAttribute>().FirstOrDefault();
                if (attr.Keys != null && attr.Keys.Any())
                {
                    attr.Keys.ToList().ForEach(c=>ObjectFactory.Register(attr.InterFaceType, type,c));
                }
                else
                {
                    ObjectFactory.Register(attr.InterFaceType, type);
                }
            }
        }
    }
}

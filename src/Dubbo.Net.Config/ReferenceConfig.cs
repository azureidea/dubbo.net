using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Config
{
    public class ReferenceConfig
    {
        /// <summary>
        /// 根据服务接口生成消费端URL
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="registry"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public static URL Init(Type interfaceType,RegistryConfig registry,ProtocolConfig protocol)
        {
            var dic = new Dictionary<string, string>();
            dic.Add(Constants.SideKey,Constants.ConsumerSide);
            dic.Add(Constants.DubboVersionKey,Common.Version.GetVersion());
            dic.Add(Constants.TimestampKey,DateTime.Now.ToTimestamp().ToString());
            var pid= Process.GetCurrentProcess().Id;
            dic.Add(Constants.PidKey,pid.ToString());
            var methods = interfaceType.GetMethods().Select(m => m.Name).ToList();
            var methodNames =methods.Count==0?"*": string.Join(",", methods);
            dic.Add("methods",methodNames);
            var interfaceName = ReflectUtil.GetJavaTypeName(interfaceType);
            dic.Add(Constants.InterfaceKey,interfaceName);
            dic.Add("protocol",protocol.Name);
            dic.Add("registry.address",registry.Address);
            dic.Add("registry.protocol", registry.Protocol);
            dic.Add("send.reconnect", "true");

            return new URL(protocol.Name,((DnsEndPoint)NetUtils.GetLocalAddress()).Host,0,interfaceName,dic);
        }
    }
}

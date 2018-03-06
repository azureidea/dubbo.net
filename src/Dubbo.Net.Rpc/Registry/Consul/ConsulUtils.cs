using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Dubbo.Net.Common;

namespace Dubbo.Net.Rpc.Registry.Consul
{
    public class ConsulUtils
    {

        /**
         * 根据服务的url生成consul对应的service
         *
         * @param url
         * @return
         */
        public static ConsulService BuildService(URL url)
        {
            ConsulService service = new ConsulService
            {
                Address = url.Ip,
                Id = ConvertServiceId(url),
                Name = url.ServiceName,
                Port = url.Port,
                Ttl = 30,
            };
            List<string> tags = new List<string>
            {
                "protocol_" + url.Protocol,
                "URL_" + url.ToString(),
                "nodeType_" + url.GetParameter(Constants.SideKey, "")
            };
            service.Tags = tags;

            return service;
        }

        /**
         * 根据service生成URL
         *
         * @param service
         * @return
         */
        public static URL BuildUrl(ConsulService service)
        {
            URL url = null;
            foreach (var tag in service.Tags)
            {
                if (tag.StartsWith("URL_"))
                {
                    var encodeUrl = tag.Substring(tag.IndexOf("_") + 1);
                    url = URL.ValueOf(URL.Decode(encodeUrl));
                }
            }
            if (url == null)
            {
                var param = new Dictionary<string, string>();
                var group = service.Name.Substring("providers_".Length);
                param.Add(Constants.GroupKey, group);
                param.Add("nodeType", service.Tags[2]);
                string protocol = ConsulUtils.GetProtocolFromTag(service.Tags[0]);
                url = new URL(protocol, service.Address, service.Port,
                        ConsulUtils.GetPathFromServiceId(service.Id), param);
            }
            return url;
        }

        /**
         * 根据url获取cluster信息，cluster 信息包括协议和path（rpc服务中的接口类）。
         *
         * @param url
         * @return
         */
        public static string GetUrlClusterInfo(URL url)
        {
            return url.Protocol + "-" + url.Path;
        }

        /**
         * 有motan的group生成consul的serivce name
         *
         * @param group
         * @return
         */
        public static string ConvertGroupToServiceName(string group)
        {
            return "providers_" + group;
        }

        /**
         * 从consul的service name中获取motan的group
         *
         * @param group
         * @return
         */
        public static string GetGroupFromServiceName(string group)
        {
            return group.Substring("providers_".Length);
        }

        //    /**
        //     * 根据motan的url生成consul的serivce id。 serviceid 包括ip＋port＋rpc服务的接口类名
        //     *
        //     * @param url
        //     * @return
        //     */
        //    public static string convertConsulSerivceId(URL url) {
        //        if (url == null) {
        //            return null;
        //        }
        //        return convertServiceId(url.getHost(), url.getPort(), url.getPath());
        //    }

        /**
         * 从consul 的serviceid中获取rpc服务的接口类名（url的path）
         *
         * @param serviceId
         * @return
         */
        public static string GetPathFromServiceId(string serviceId)
        {
            return serviceId.Substring(serviceId.IndexOf("-") + 1);
        }

        /**
         * 从consul的tag获取motan的protocol
         *
         * @param tag
         * @return
         */
        public static string GetProtocolFromTag(string tag)
        {
            return tag.Substring("protocol_".Length);
        }


        public static string ConvertServiceId(URL url)
        {
            try
            {
                return url.GetParameter(Constants.SideKey,Constants.ProviderSide) + "_" + url.Protocol + "_" + url.Ip
                        + "_" + url.Port+ "_" + Process.GetCurrentProcess().Id;
            }
            catch (Exception e)
            {
                return "";
            }

        }


}
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Dubbo.Net.Common.Utils
{
    public class NetUtils
    {
        private static  EndPoint LocalAddress;
        /**
     * Find first valid IP from local network card
     *
     * @return first valid local IP
     */
        public static EndPoint GetLocalAddress()
        {
            if (LocalAddress != null)
                return LocalAddress;
            var localAddress = GetLocalAddress0();
            LocalAddress = localAddress;
            return localAddress;
        }
        private static EndPoint GetLocalAddress0()
        {
            EndPoint localAddress = null;
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            foreach (IPAddress ipa in ipadrlist)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                {
                    localAddress=new DnsEndPoint(ipa.ToString(),0);
                }
            }
            return localAddress;
        }
    }
}

using Dubbo.Net.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dubbo.Net.Test
{
    [JavaName("com.mc.userconnect.api.contract.request.UserSyncNotify")]
    public class UserSyncNotify
    {
        public string version { get; set; }
        public string merchantId { get; set; }
        public string mid { get; set; }
        public string uid { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public string signature { get; set; }
    }
}

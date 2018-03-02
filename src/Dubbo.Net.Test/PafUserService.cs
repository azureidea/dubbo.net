using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Proxy;
using Dubbo.Net.Test.Models;

namespace Dubbo.Net.Test
{
    public  class PafUserService:ServiceProxyBase,IPafUserService
    {
        public PafUserService(URL url) : base(url)
        {
        }
        [Refer("getAccessToken")]
        public Task<UserConnectStringResponse> GetAccessToken(long mallId, long uid)
        {
            var method = this.GetType().GetMethod("GetAccessToken");
            var args = new object[] {mallId, uid,};
            return base.Invoke<UserConnectStringResponse>(method, args);
        }
    }
}

using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Attributes;
using Dubbo.Net.CoreTest.Models;
using Dubbo.Net.Proxy;

namespace Dubbo.Net.CoreTest
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

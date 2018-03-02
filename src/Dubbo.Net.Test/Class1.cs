using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Proxy;
using Dubbo.Net.Test;
using Dubbo.Net.Test.Models;
    namespace Dubbo.Net.ClientProxys
{
public class PafUserServiceProxy : ServiceProxyBase, IPafUserService
{
    public PafUserServiceProxy(URL url) : base(url)
    { }
    public Task<Dubbo.Net.Test.Models.UserConnectStringResponse> GetAccessToken(System.Int64 pram0, System.Int64 pram1)
    {
        var method = this.GetType().GetMethod("GetAccessToken");
        var args = new object[] { pram0, pram1, };
        return base.Invoke<Dubbo.Net.Test.Models.UserConnectStringResponse>(method, args);
    }
}
}

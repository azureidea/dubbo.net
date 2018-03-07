using System.Threading.Tasks;
using Dubbo.Net.Common.Attributes;
using Dubbo.Net.CoreTest.Models;

namespace Dubbo.Net.CoreTest
{
    [Refer("com.mc.userconnect.api.service.PafUserService")]
    public interface IPafUserService
    {
        [Refer("getAccessToken")]
        Task<UserConnectStringResponse> GetAccessToken(long mallId, long uid);

        //[TypeName("userSync")]
        //Task<UserSyncResponse> UserSync(UserSyncNotify notify);
        //[TypeName("memberSync")]
        //Task<UserSyncResponse> MemberSync(MemberSyncNotify notify);
        //[TypeName("getPafAccessTokenV2")]
        //Task<UserConnectStringResponse> GetPafAccessTokenV2(long mallId, long uid);
        //[TypeName("getServiceUrl")]
        //Task<UserConnectStringResponse> GetServiceUrl(PafUserTokenRequest request);
        //[TypeName("getRefreshToken")]
        //Task<RefreshTokenResponse> GetRefreshToken(PafRefreshTokenRequest request);
    }
}

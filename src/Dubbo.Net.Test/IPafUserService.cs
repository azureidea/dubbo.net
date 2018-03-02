using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Test.Models;

namespace Dubbo.Net.Test
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

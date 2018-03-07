using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Remoting.Exchange.Support.Header
{
    [DependencyIoc(typeof(IExchanger),"header")]
    public class HeaderExchanger:IExchanger
    {
        public Task<IExchangeServer> BindAsync(URL url, IExchangeHandler handler)
        {
            return null;
        }

        public async Task<IExchangeClient> ConnectAsync(URL url, IExchangeHandler handler)
        {
            var transport = ObjectFactory.GetInstance<ITransporter>(url.GetParameter("transtport", "netty"));
            //todo 
            return await Task.FromResult(new HeaderExchangeClient(transport.Connected(url,handler),true));
        }
    }
}

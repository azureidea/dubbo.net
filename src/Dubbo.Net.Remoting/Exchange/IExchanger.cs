using System.Threading.Tasks;
using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Exchange
{
    public interface IExchanger
    {
        Task<IExchangeServer> BindAsync(URL url, IExchangeHandler handler);
        Task<IExchangeClient> ConnectAsync(URL url,IExchangeHandler handler);
    }
}

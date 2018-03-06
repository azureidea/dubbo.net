using System.Threading.Tasks;
using Dubbo.Net.Remoting.Telnet;

namespace Dubbo.Net.Remoting.Exchange.Support
{
    public abstract class ExchangeHandlerAdapter:TelnetHandlerAdapter,IExchangeHandler
    {
        public virtual Task<object> Reply(IExchangeChannel channel, object request)
        {
            return null;
        }
    }
}

using System.Threading.Tasks;

namespace Dubbo.Net.Remoting.Exchange.Support
{
    public interface IReplier
    {
        Task<object> ReplyAsync(IExchangeChannel channel, object request);
    }
}

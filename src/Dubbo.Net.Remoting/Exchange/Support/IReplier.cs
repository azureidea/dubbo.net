using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dubbo.Net.Remoting.Exchange.Support
{
    public interface IReplier
    {
        Task<object> ReplyAsync(IExchangeChannel channel, object request);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Remoting.Telnet;

namespace Dubbo.Net.Remoting.Exchange
{
    public interface IExchangeHandler:IChannelHandler,ITelnetHandler
    {
        Task<object> Reply(IExchangeChannel channel, object request);
    }
}

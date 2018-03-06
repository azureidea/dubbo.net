using System.Collections.Generic;
using System.Net;

namespace Dubbo.Net.Remoting.Exchange
{
    public interface IExchangeServer:IServer
    {
        ICollection<IExchangeChannel> GetExchangeChannels();
        IExchangeChannel GetExchangeChannel(EndPoint address);

    }
}

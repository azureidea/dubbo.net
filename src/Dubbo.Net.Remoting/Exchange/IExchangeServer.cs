using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Dubbo.Net.Remoting.Exchange
{
    public interface IExchangeServer:IServer
    {
        ICollection<IExchangeChannel> GetExchangeChannels();
        IExchangeChannel GetExchangeChannel(EndPoint address);

    }
}

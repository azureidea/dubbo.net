using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Exchange
{
    public interface IExchangeChannel:IChannel
    {
        Task<Response> Request(object request);
        Task<Response> Request(object request, int timeout);
        IExchangeHandler GetExchangeHandler();
    }
}

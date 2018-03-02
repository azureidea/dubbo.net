using Dubbo.Net.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dubbo.Net.Remoting
{
    public interface IEndpoint
    {
        URL Url { get; }
        IChannelHandler ChannelHander { get; }
        EndPoint Address { get; }
        Task<Response> SendAsync(object message);
        Task<Response> SendAsync(object message, bool sent);
        Task CloseAsync();
        Task CloseAsync(int timeout);
        bool IsClosed { get;  }
    }
}

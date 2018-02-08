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
        URL Url { get; set; }
        IChannelHandler ChannelHander { get; set; }
        EndPoint Address { get; set; }
        Task SendAsync(object message);
        Task SendAsync(object message, bool sent);
        Task CloseAsync();
        Task CloseAsync(int timeout);
        bool IsClosed { get; set; }
    }
}

using Dubbo.Net.Common;
using System.Collections.Generic;
using System.Net;

namespace Dubbo.Net.Remoting
{
    public interface IServer:IEndpoint,IResetable
    {
        bool IsBound();
        ICollection<IChannel> GetChannels();
        IChannel GetChannel(IPAddress remoteAddress);

    }
}

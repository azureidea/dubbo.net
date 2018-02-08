using Dubbo.Net.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Dubbo.Net.Remoting
{
    public interface IServer:IEndpoint,IResetable
    {
        bool IsBound();
        ICollection<IChannel> GetChannels();
        IChannel GetChannel(IPAddress remoteAddress);

    }
}

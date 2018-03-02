using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Remoting.Transport
{
    public interface IChannelHandlerDelegate:IChannelHandler
    {
        IChannelHandler GetHandler();
    }
}

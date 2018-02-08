using Dubbo.Net.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Remoting
{
    public interface IDispatcher
    {
        IChannelHandler Dispatch(IChannelHandler handler, URL url);
    }
}

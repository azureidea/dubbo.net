using Dubbo.Net.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Remoting
{
    public interface ITransporter
    {
        IServer Bind(URL url, IChannelHandler handler);
        IClient Connected(URL url, IChannelHandler handler);
    }
}

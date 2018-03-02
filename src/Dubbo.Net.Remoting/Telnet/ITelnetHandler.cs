using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Remoting.Telnet
{
    public interface ITelnetHandler
    {
        string Telnet(IChannel channel, string msg);
    }
}

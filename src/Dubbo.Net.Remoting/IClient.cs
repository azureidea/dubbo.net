using Dubbo.Net.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Remoting
{
    public interface IClient:IEndpoint,IChannel,IResetable
    {
        void Reconnect();

    }
}

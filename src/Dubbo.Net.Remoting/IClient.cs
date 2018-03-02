using Dubbo.Net.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dubbo.Net.Remoting
{
    public interface IClient:IChannel,IResetable
    {
        Task ReconnectAsync();
        
    }
}

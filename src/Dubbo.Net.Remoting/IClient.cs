using Dubbo.Net.Common;
using System.Threading.Tasks;

namespace Dubbo.Net.Remoting
{
    public interface IClient:IChannel,IResetable
    {
        Task ReconnectAsync();
        
    }
}

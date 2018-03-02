using Dubbo.Net.Common;

namespace Dubbo.Net.Rpc.Infrastructure
{
    public interface INode
    {
        URL GetUrl();
        bool IsAvailable();
        void Destroy();
    }
}

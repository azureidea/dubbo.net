using System;
using System.Threading.Tasks;
using Dubbo.Net.Rpc.Infrastructure;

namespace Dubbo.Net.Rpc
{
    public interface IInvoker:INode
    {
        string InvokerId { get; set; }
        Type GetInterface();
        Task<IResult> Invoke(IInvocation invocation);
    }
}

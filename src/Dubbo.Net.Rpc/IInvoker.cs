using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Rpc.Infrastructure;

namespace Dubbo.Net.Rpc
{
    public interface IInvoker:INode
    {
        Type GetInterface();
        Task<IResult> Invoke(IInvocation invocation);
    }
}

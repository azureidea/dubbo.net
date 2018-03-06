using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Rpc;
using Dubbo.Net.Rpc.Infrastructure;
using Dubbo.Net.Rpc.Support;

namespace Dubbo.Net.Proxy
{
    public abstract class ServiceProxyBase
    {
        private readonly IProtocol _protocol;
        private  int _invokeCount=0;
        private IInvoker Get_invokers()
        {
            return _protocol.Invokers[Interlocked.Increment(ref _invokeCount)%_protocol.Invokers.Count];
        }

        protected ServiceProxyBase(URL url)
        {
            var protocolName = url.Protocol;
            _protocol = ObjectFactory.GetInstance<IProtocol>(protocolName);
            _protocol.Refer(url);
        }

        protected async Task<T> Invoke<T>(MethodInfo method,object[] args)
        {
            var inv = new RpcInvocation(method, args);
            inv.ReturnType = typeof(T);
            //todo 这里可以添加负载均衡策略、客户端过滤器等
            var result= await Get_invokers().Invoke(inv);
            if (result.HasException||result.Exception!=null)
                throw result.Exception;
            var value= ((Response) result.Value).Mresult;
            return (T)((RpcResult)value).Value;
        }
    }
}

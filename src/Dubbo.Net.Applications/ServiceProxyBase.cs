using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Rpc;
using Dubbo.Net.Rpc.Infrastructure;
using Newtonsoft.Json;

namespace Dubbo.Net.Applications
{
    public abstract class ServiceProxyBase
    {
        private readonly IProtocol _protocol;
        private int _invokeCount = 0;
        private readonly string _serviceName;
        private IInvoker GetInvoker()
        {
            var invokers = _protocol.Invokers(_serviceName);
            if (invokers.Count == 0)
            {
                return null;
            }
            return invokers[Interlocked.Increment(ref _invokeCount) % invokers.Count];
        }

        protected ServiceProxyBase(URL url)
        {
            var protocolName = url.Protocol;
            _protocol = ObjectFactory.GetInstance<IProtocol>(protocolName);
            _serviceName = url.ServiceName;
            //_protocol.Refer(url);
        }

        protected async Task<T> Invoke<T>(MethodInfo method, object[] args)
        {
            //Console.WriteLine("step1:"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            var inv = new RpcInvocation(method, args);
            //JsonConvert.SerializeObject(inv);
            //Console.WriteLine("step2:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            inv.ReturnType = typeof(T);
            //todo 这里可以添加负载均衡策略、客户端过滤器等
            IResult result = null;
            var invoker = GetInvoker();
            //Console.WriteLine("step3:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            if (invoker == null)
                throw new Exception("no service found!");
            result = await invoker.Invoke(inv);
           // Console.WriteLine("step12:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            if (result.HasException  || result.Exception != null)
                throw result.Exception;
            var value = ((Response)result.Value).Mresult;
            //Console.WriteLine("step13:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            return (T)((RpcResult)value).Value;
        }
    }
}

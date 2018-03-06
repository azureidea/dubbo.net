using System.Collections.Concurrent;

namespace Dubbo.Net.Rpc.Support
{
    public static class InvocationUtils
    {
        private static readonly ConcurrentDictionary<long,object> _invocations=new ConcurrentDictionary<long, object>();

        public static object GetInvocation(long id)
        {
            if (_invocations.TryRemove(id, out var invocation))
                return invocation;
            return null;
        }

        public static void SetInvocation(long id, object invocation)
        {
            _invocations.TryAdd(id, invocation);
        }
    }
}

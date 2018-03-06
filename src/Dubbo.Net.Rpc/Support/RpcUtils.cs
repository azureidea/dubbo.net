using System;
using System.Reflection;
using System.Threading;
using Dubbo.Net.Common;

namespace Dubbo.Net.Rpc.Support
{
    public class RpcUtils
    {
        private static  int _invokeId = 0;

        public static Type GetReturnType(IInvocation invocation)
        {
            try
            {
                if (invocation != null && invocation.Invoker != null
                        && invocation.Invoker.GetUrl() != null
                        && !invocation.MethodName.StartsWith("$"))
                {
                    string service = invocation.Invoker.GetUrl().ServiceName;
                    if (!string.IsNullOrEmpty(service))
                    {
                        Type cls = ReflectUtil.ForName(service);
                        MethodInfo method = cls.GetMethod(invocation.MethodName, invocation.ParameterTypes);
                        if (method.ReturnType == typeof(void))
                        {
                            return null;
                        }

                        return method.ReturnType;
                    }
                }
            }
            catch (Exception t)
            {
                //
            }
            return null;
        }

        public static Type[] GetReturnTypes(IInvocation invocation)
        {
            try
            {
                if (invocation?.ReturnType != null)
                    return new[] {invocation.ReturnType};
                if (invocation?.Invoker?.GetUrl() != null && !invocation.MethodName.StartsWith("$"))
                {
                    string service = invocation.Invoker.GetUrl().ServiceName;
                    if (!string.IsNullOrEmpty(service))
                    {
                        Type cls = ReflectUtil.ForName(service);
                        MethodInfo method = cls.GetMethod(ReflectUtil.GetCsMethodName(invocation.MethodName), invocation.ParameterTypes);
                        if (method.ReturnType == typeof(void))
                        {
                            return null;
                        }

                        return new Type[] { method.ReturnType };
                    }
                }
            }
            catch (Exception t)
            {
                // ignored
            }

            return null;
        }

        public static long GetInvocationId(IInvocation inv)
        {
            var id = inv.GetAttachment(Constants.IdKey);
            long.TryParse(id, out long result);
            return result;
        }
        
        /**
         * Idempotent operation: invocation id will be added in async operation by default
         *
         * @param url
         * @param inv
         */
        public static void AttachInvocationIdIfAsync(URL url, IInvocation inv)
        {
            if (IsAttachInvocationId(url, inv) && GetInvocationId(inv) == 0) {
                (inv as RpcInvocation)?.SetAttachment(Constants.IdKey, Interlocked.Increment(ref _invokeId).ToString());
            }
        }

        private static bool IsAttachInvocationId(URL url, IInvocation invocation)
        {
            var value = url.GetMethodParameter(invocation.MethodName, Constants.AutoAttachInvocationIdKey);
            if (value == null)
            {
                // add invocationid in async operation by default
                return IsAsync(url, invocation);
            }
            else if ("true".Equals(value,StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetMethodName(IInvocation invocation)
        {
            if (Constants.Invoke.Equals(invocation.MethodName)&& invocation.Arguments != null&& invocation.Arguments.Length > 0
                    && (invocation.Arguments[0] is string))
            {
                return (string)invocation.Arguments[0];
            }
            return invocation.MethodName;
        }

        public static object[] GetArguments(IInvocation invocation)
        {
            if (Constants.Invoke.Equals(invocation.MethodName)
                    && invocation.Arguments != null
                    && invocation.Arguments.Length > 2
                    && invocation.Arguments[2] is object[]) {
                return (object[])invocation.Arguments[2];
            }
            return invocation.Arguments;
        }

        public static Type[] GetParameterTypes(IInvocation invocation)
        {
            if (Constants.Invoke.Equals(invocation.MethodName)&& invocation.Arguments != null
                    && invocation.Arguments.Length > 1
                    && invocation.Arguments[1] is string[]) {
                string[] types = (string[])invocation.Arguments[1];
                if (types == null)
                {
                    return new Type[0];
                }
                Type[] parameterTypes = new Type[types.Length];
                for (int i = 0; i < types.Length; i++)
                {
                    parameterTypes[i] = ReflectUtil.ForName(types[0]);
                }
                return parameterTypes;
            }
            return invocation.ParameterTypes;
        }

        public static bool IsAsync(URL url, IInvocation inv)
        {
            bool isAsync;
            if ("true".Equals(inv.GetAttachment(Constants.AsyncKey)))
            {
                isAsync = true;
            }
            else
            {
                isAsync = url.GetMethodParameter(GetMethodName(inv), Constants.AsyncKey,  false);
            }
            return isAsync;
        }

        public static bool IsOneway(URL url, IInvocation inv)
        {
            bool isOneway;
            if ("false".Equals(inv.GetAttachment(Constants.ReturnKey)))
            {
                isOneway = true;
            }
            else
            {
                isOneway = !url.GetMethodParameter(GetMethodName(inv), Constants.ReturnKey,  true);
            }
            return isOneway;
        }
    }
}

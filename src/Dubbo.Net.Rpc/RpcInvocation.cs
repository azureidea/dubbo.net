using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dubbo.Net.Common;

namespace Dubbo.Net.Rpc
{
    public class RpcInvocation:IInvocation
    {
        /// <summary>
        /// 调用的方法
        /// </summary>
        public string MethodName { get; set; }

        public Type[] ParameterTypes { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public object[] Arguments { get; set; }

        public Type ReturnType { get; set; }

        public Dictionary<string, string> Attachments { get; set; }
        public RpcInvocation() { }

        public RpcInvocation(MethodInfo method, object[] arguments):this(ReflectUtil.GetMethodName(method),method.GetParameters().Select(p=>p.ParameterType).ToArray(),arguments)
        {
        }

        public RpcInvocation(string methodName, Type[] parameterTypes, object[] arguments, Dictionary<string,string> attachments=null, IInvoker invoker=null)
        {
            MethodName = methodName;
            ParameterTypes = parameterTypes ?? new Type[0];
            Arguments = arguments ?? new object[0];
            Attachments = attachments ?? new Dictionary<string, string>();
            Invoker = invoker;
        }

        public string GetAttachment(string key,string defaultValue = null)
        {
            if (Attachments == null || !Attachments.ContainsKey(key))
            {
                return defaultValue;
            }
            var value = Attachments[key];
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            return value;
        }

        public IInvoker Invoker { get; set; }

        public void SetAttachment(string key, string value)
        {
            if (Attachments == null)
            {
                Attachments = new Dictionary<string, string>();
            }
            Attachments.Add(key, value);
        }
        public void AddAttachmentsIfAbsent(Dictionary<string, string> attachments)
        {
            if (attachments == null)
            {
                return;
            }
            foreach (var entry in attachments)
            {
                if (!Attachments.ContainsKey(entry.Key))
                {
                    Attachments.Add(entry.Key,entry.Value);
                }
            }
        }
        public override string ToString()
        {
            return "RpcInvocation [_methodName=" + MethodName + ", _parameterTypes="
                     + string.Join(",", ParameterTypes.ToList()) + ", _arguments=" + string.Join(",", Arguments.ToList())
                     + ", _attachments=" + Attachments + "]";
        }
    }
}

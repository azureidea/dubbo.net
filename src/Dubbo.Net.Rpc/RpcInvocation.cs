using System;
using System.Collections.Generic;
using System.Linq;

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

        public Dictionary<string, string> Attachments { get; set; }



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
        public override string ToString()
        {
            return "RpcInvocation [_methodName=" + MethodName + ", _parameterTypes="
                     + string.Join(",", ParameterTypes.ToList()) + ", _arguments=" + string.Join(",", Arguments.ToList())
                     + ", _attachments=" + Attachments + "]";
        }
    }
}

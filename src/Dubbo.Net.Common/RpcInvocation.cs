using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dubbo.Net.Common
{
    public class RpcInvocation
    {
        /// <summary>
        /// 调用的方法
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 参数类型列表
        /// </summary>
        public Type[] ParameterTyes { get; set; }
        /// <summary>
        /// 参数列表
        /// </summary>
        public object[] Arguments { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public Dictionary<string,string> Attchments { get; set; }
        

        public string GetAttachment(string key,string defaultValue = null)
        {
            if (Attchments == null || !Attchments.ContainsKey(key))
            {
                return defaultValue;
            }
            var value = Attchments[key];
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            return value;
        }

        public override string ToString()
        {
            return "RpcInvocation [_methodName=" + MethodName + ", _parameterTypes="
                     + string.Join(",", ParameterTyes.ToList()) + ", _arguments=" + string.Join(",", Arguments.ToList())
                     + ", _attachments=" + Attchments + "]";
        }
    }
}

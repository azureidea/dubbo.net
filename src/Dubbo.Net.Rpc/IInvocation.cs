using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Rpc
{
    public interface IInvocation
    {
        string MethodName { get; set; }
        Type[] ParameterTypes { get; set; }
        object[] Arguments { get; set; }
        Type ReturnType { get; set; }
        Dictionary<string,string> Attachments { get; set; }
        string GetAttachment(string key, string defaultValue = "");
        IInvoker Invoker { get; set; }
    }
}

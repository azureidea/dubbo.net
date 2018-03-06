using System;
using System.Collections.Generic;

namespace Dubbo.Net.Rpc
{
    public class RpcResult : IResult
    {
        private const long SerialVersionUid = -6925924956850004727L;
        public RpcResult()
        {
        }

        public RpcResult(object result)
        {
            this.Value = result;
        }

        public RpcResult(Exception exception)
        {
            this.Exception = exception;
        }

        public object Value { get; set; }
        public Exception Exception { get; set; }

        public bool HasException => Exception != null;

        public object Recreate()
        {
            if (Exception != null)
            {
                throw Exception;
            }
            return Value;
        }

        public Dictionary<string, string> Attachments { get; set; }

   





        public string GetAttachment(string key, string defaultValue)
        {
            Attachments = Attachments ?? new Dictionary<string, string>();
            Attachments.TryGetValue(key, out string result);
            if (string.IsNullOrEmpty(result))
            {
                result = defaultValue;
            }
            return result;
        }

        public void SetAttachment(string key, string value)
        {
            Attachments = Attachments ?? new Dictionary<string, string>();
            Attachments.Add(key, value);
        }

        public override string ToString()
        {
            return "RpcResult [result=" + Value + ", exception=" + Exception + "]";
        }
    }
}

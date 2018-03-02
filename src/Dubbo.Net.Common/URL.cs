using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace Dubbo.Net.Common
{
    public class URL
    {
        public int Port { get; set; }
        public string Ip { get; set; }
        public string ServiceName { get; set; }
        public string Path { get; set; }
        private readonly Dictionary<string, string> _parameters=new Dictionary<string, string>();
        public static URL ValueOf(string url)
        {
            return new URL();
        }

        public T GetParameter<T>(string key, T defaultValue )
        {
            if (!_parameters.ContainsKey(key))
                return defaultValue;
            var v = _parameters[key];
            if (string.IsNullOrEmpty(v))
                return defaultValue;
            try
            {
                var result=Convert.ChangeType(v, typeof(T));
                return (T)result;
            }
            catch (Exception e)
            {
                return defaultValue;
            }
        }
        public string GetMethodParameter(string method, string key)
        {
            var id = $"{method}.{key}";
            string value="";
            if (_parameters.ContainsKey(id))
                return _parameters[id];
            return value;
        }

        public string GetMethodParameter(string method, string key, string defaultValue)
        {
            string value = GetMethodParameter(method, key);
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            return value;
        }
        public bool GetMethodParameter(string method, string key, bool defaultValue)
        {
            string value = GetMethodParameter(method, key);
            var result = false;
            if (string.IsNullOrEmpty(value)&&!bool.TryParse(value,out  result))
            {
                return defaultValue;
            }
            return result;
        }
        public bool HasParameter(String key)
        {
            String value = GetParameter<string>(key,"");
            return !string.IsNullOrEmpty(value);
        }
        public int GetPositiveParameter(String key, int defaultValue)
        {
            if (defaultValue <= 0)
            {
                throw new Exception("defaultValue <= 0");
            }
            int value = GetParameter(key, defaultValue);
            if (value <= 0)
            {
                return defaultValue;
            }
            return value;
        }

        public EndPoint ToInetSocketAddress()
        {
            return new DnsEndPoint(Ip,Port);
        }
        public URL AddParameterIfAbsent(string key, string value)
        {
            if (string.IsNullOrEmpty(key)||string.IsNullOrEmpty(value))
            {
                return this;
            }
            if (HasParameter(key))
            {
                return this;
            }
            _parameters.Add(key,value);
            //todo 生成新的URL并加入key value
            return this;
        }

        public URL AddParameter(string key, object value)
        {
            value = value ?? "";
            _parameters.Add(key,value.ToString());
            return this;
        }

        public string GetAddress()
        {
            return Ip + (Port <= 0 ? "" : ":" + Port);
        }

        public string GetParameterAndDecoded(string key, string defaultValue)
        {
            return Decode(GetParameter(key, defaultValue));
        }

        public static string Decode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            return HttpUtility.UrlDecode(value, Encoding.UTF8);
        }
    }
}

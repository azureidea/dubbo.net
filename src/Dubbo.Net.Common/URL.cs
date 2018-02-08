using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common
{
    public class URL
    {
        public int Port { get; set; }
        public string Ip { get; set; }
        public string ServiceName { get; set; }
        private readonly Dictionary<string, string> _parameters=new Dictionary<string, string>();
        public static URL ValueOf(string url)
        {
            return new URL();
        }

        public T GetParameter<T>(string key, T defaultValue )
        {
            return default(T);
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
    }
}

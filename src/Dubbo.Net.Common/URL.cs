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
        public string Protocol { get; set; }
        private readonly Dictionary<string, string> _parameters=new Dictionary<string, string>();
        public static URL ValueOf(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("url == null");
            }
            String protocol = null;
            String username = null;
            String password = null;
            String host = null;
            int port = 0;
            String path = null;
            Dictionary<String, String> parameters = null;
            int i = url.IndexOf("?", StringComparison.Ordinal); // seperator between body and parameters 
            if (i >= 0)
            {
                String[] parts = url.Substring(i + 1).Split('&');
                parameters = new Dictionary<string, string>();
                foreach (var part in parts)
                {
                    if (!string.IsNullOrWhiteSpace(part))
                    {
                        int j = part.IndexOf('=');
                        if (j >= 0)
                        {
                            parameters.Add(part.Substring(0, j), part.Substring(j + 1));
                        }
                        else
                        {
                            parameters.Add(part, part);
                        }
                    }
                }
                url = url.Substring(0, i);
            }
            i = url.IndexOf("://", StringComparison.Ordinal);
            if (i >= 0)
            {
                if (i == 0) throw new Exception("url missing protocol: \"" + url + "\"");
                protocol = url.Substring(0, i);
                url = url.Substring(i + 3);
            }
            else
            {
                // case: file:/path/to/file.txt
                i = url.IndexOf(":/", StringComparison.Ordinal);
                if (i >= 0)
                {
                    if (i == 0) throw new Exception("url missing protocol: \"" + url + "\"");
                    protocol = url.Substring(0, i);
                    url = url.Substring(i + 1);
                }
            }

            i = url.IndexOf("/", StringComparison.Ordinal);
            if (i >= 0)
            {
                path = url.Substring(i + 1);
                url = url.Substring(0, i);
            }
            i = url.IndexOf("@", StringComparison.Ordinal);
            if (i >= 0)
            {
                username = url.Substring(0, i);
                int j = username.IndexOf(":", StringComparison.Ordinal);
                if (j >= 0)
                {
                    password = username.Substring(j + 1);
                    username = username.Substring(0, j);
                }
                url = url.Substring(i + 1);
            }
            i = url.IndexOf(":", StringComparison.Ordinal);
            if (i >= 0 && i < url.Length - 1)
            {
                 int.TryParse(url.Substring(i + 1),out port);
                url = url.Substring(0, i);
            }
            if (url.Length> 0) host = url;
            return new URL(protocol,username,password,  host, port, path, parameters);
        }
        public URL() { }

        public URL(string protocol, string host, int port, string path, Dictionary<string, string> parameters):this(protocol,null,null,host,port,path,parameters)
        {
        }

        public URL(string protocol, string userName,string password,string host, int port,string path, Dictionary<string, string> parameters)
        {
            Protocol = protocol;
            Ip = host;
            Port = port < 0 ? 0 : port;
            Path = path;
            ServiceName = path;
            _parameters = parameters ?? new Dictionary<string, string>();
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

        public string ToRegistryString()
        {
            return $"{Protocol}://{Ip}:{Port}";
        }

        public string GetId()
        {
            return GetParameter(Constants.SideKey, Constants.ProviderSide) + "_" + Protocol + "_" + Ip
                + "_" + Port + "_";
        }
    }
}

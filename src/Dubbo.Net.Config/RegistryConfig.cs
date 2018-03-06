using System;

namespace Dubbo.Net.Config
{
    public class RegistryConfig
    {
        public string Protocol { get; }
        public string Address { get; }
        private string _config;
        public RegistryConfig(string config)
        {
            var index = config.IndexOf("://", StringComparison.Ordinal);
            if(index<=0)
                throw new Exception("invalid registry url :"+config);
            Protocol = config.Substring(0, index);
            Address = config.Substring(index+3);
            _config = config;
        }

        public string ToRegistryString()
        {
            return _config;
        }
    }
}

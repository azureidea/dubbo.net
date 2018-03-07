using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Rpc.Infrastructure;

namespace Dubbo.Net.Rpc.Registry.Consul
{
    public class ConsulRegistry:IRegistry
    {
        private readonly ConsulEcwidClient _client;
        private readonly URL _url;
        private readonly ConsulHeartbeatManager _heartbeat;
        private readonly CancellationTokenSource _source;
        readonly ConcurrentDictionary<string,string> _serviceNames=new ConcurrentDictionary<string, string>();
        public ConsulRegistry(URL url)
        {
            var address = $"{url.Ip}:{url.Port}";
            if(string.IsNullOrEmpty(address))
                throw new Exception("Invalid registry address ");
            _client=new ConsulEcwidClient(address);
            _url = url;
            _heartbeat=new ConsulHeartbeatManager(_client);
            _heartbeat.Start();
            _source=new CancellationTokenSource();
        }


        public URL GetUrl()
        {
            return _url;
        }

        public bool IsAvailable()
        {
            return _client.IsConnected;
        }

        public void Destroy()
        {
            _source.Cancel();
        }

        public async Task Register(URL url)
        {
            var service=new ConsulService(url);
            await _client.RegisterService(service);
            await _client.CheckPass(service.Id);
            _heartbeat.AddServiceId(service.Id);
        }

        public async Task UnRegister(string  serviceId)
        {
            await _client.UnRegisterService(serviceId);
            _heartbeat.RemoveServiceId(serviceId);
        }

        public  Task Subscribe(string serviceName)
        {
            _serviceNames.TryAdd(serviceName, serviceName);
             return Task.CompletedTask;
        }

        public async Task<List<URL>> Lookup(string serviceName)
        {
           
            try
            {
               // ConsulService service = ConsulUtils.BuildService(url);
                ConsulResponse<List<ConsulService>> response = await _client.LookupHealthService(serviceName, 0L);
                List<ConsulService> services = response.Value;
                List<URL> urls = new List<URL>(services.Count);
                foreach (ConsulService service1 in services)
                {
                    urls.Add(ConsulUtils.BuildUrl(service1));
                }
                return urls;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to lookup " + serviceName + " from consul " + GetUrl() + ", cause: " + e.Message, e);
            }
        }
        public void Start()
        {
             RefreshService().Wait();
            Task.Run(async () =>
            {
                while (!_source.IsCancellationRequested)
                {
                    
                    await Task.Delay(TimeSpan.FromSeconds(25));
                }
            }, _source.Token);
        }

        private async Task RefreshService()
        {
            foreach (var serviceId in _serviceNames)
            {
                var urls = await Lookup(serviceId.Key);
                foreach (var url in urls)
                {
                    var protocol = ObjectFactory.GetInstance<IProtocol>(url.Protocol);
                    protocol.Refer(url);
                }
            }
        }
        public Task UnSubscribe(string serviceName)
        {
            _serviceNames.TryRemove(serviceName, out var value);
            return Task.CompletedTask;
        }
    }
}

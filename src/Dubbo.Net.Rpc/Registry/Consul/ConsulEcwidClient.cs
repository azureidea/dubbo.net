using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consul;

namespace Dubbo.Net.Rpc.Registry.Consul
{
    public class ConsulEcwidClient
    {
        private readonly ConsulClient _client;
        public ConsulEcwidClient(string consulString) 
        {
            _client=new ConsulClient(config =>
            {
                config.Address = new Uri($"http://{consulString}");
            });
            
        }

        public async  Task CheckPass(string serviceId)
        {
            await _client.Agent.CheckRegister(new AgentCheckRegistration {ServiceID = serviceId});
        }

        public  Task CheckFail(string serviceId)
        {
            return _client.Agent.CheckDeregister(serviceId);
        }

        public  Task RegisterService(ConsulService service)
        {
            return _client.Agent.ServiceRegister(ConvertService(service));
        }

        public Task UnRegisterService(string serviceId)
        {
            return _client.Agent.ServiceDeregister(serviceId);
        }

        public async Task<ConsulResponse<List<ConsulService>>> LookupHealthService(string serviceName, ulong lastConsulIndex)
        {
            var queryParams = new QueryOptions
            {
                WaitIndex = lastConsulIndex,
                WaitTime = TimeSpan.FromMinutes(10)
            };
            var response=await _client.Health.Service(serviceName, "", true, queryParams);
            var consulResponse=new ConsulResponse<List<ConsulService>>();
            if (response?.Response?.Length > 0)
            {
                var healthServices = response.Response;
                var consulServices = new List<ConsulService>(healthServices.Length);
                foreach (var healthService in healthServices)
                {
                    consulServices.Add(ConverToConsulService(healthService.Service)); 
                }

                if (consulServices.Any())
                {
                    consulResponse.Value = consulServices;
                    consulResponse.ConsulIndex = response.LastIndex;
                    consulResponse.ConsulLastContact = response.LastContact;
                    consulResponse.ConsulKnownLeader=response.KnownLeader;
                }
            }

            return consulResponse;
        }

        public async Task<string> LookupCommand(string group)
        {
            var  response = await _client.KV.Get("dubbbo/command/providers_" + group);
            var value = response.Response.Value;
            String command = "";
            if (value != null)
            {
                command = Encoding.UTF8.GetString(value);
            }
            return command;
        }

        public bool IsConnected => true;

        AgentServiceRegistration ConvertService(ConsulService service)
        {
            var registration = new AgentServiceRegistration
            {
                Address = service.Address,
                ID = service.Id,
                Name = service.Name,
                Port = service.Port,
                Tags = service.Tags.ToArray()
            };
            var check = new AgentServiceCheck {TTL = TimeSpan.FromSeconds(service.Ttl)};
            registration.Check = check;
            return registration;
        }

        ConsulService ConverToConsulService(AgentService service)
        {
            return new ConsulService
            {
                Address = service.Address,
                Id = service.ID,
                Name = service.ID,
                Tags = service.Tags.ToList(),
                Port = service.Port
            };
        }
    }
}

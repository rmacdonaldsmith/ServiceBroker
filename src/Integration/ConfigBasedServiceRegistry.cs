using System;
using System.Collections.Generic;
using MHM.WinFlexOne.Services.Integration.Schema;
using ServiceBroker.Common;

namespace MHM.WinFlexOne.Services.Integration
{
    public class ConfigBasedServiceRegistry : IServiceRegistry
    {
        private readonly Dictionary<Type, ServiceDefinition>  m_serviceTypeToServiceDefinitionMap = new Dictionary<Type, ServiceDefinition>(); // key = sevice interface type
        private readonly Dictionary<Type, string> m_serviceTypeToDefinitionIdCache = new Dictionary<Type, string>(); //key = service interface type, value = service definition id
        private readonly Dictionary<string, List<ServiceInstanceInfo>> m_serviceInstanceCache = new Dictionary<string, List<ServiceInstanceInfo>>(); //key = service definition id

        public ConfigBasedServiceRegistry(ServiceRegistryConfig serviceRegistryConfig)
        {
            Ensure.ParameterIsNotNull(serviceRegistryConfig, "serviceRegistryConfig");

            //load service definition caches and service instance cache from config
            if (serviceRegistryConfig != null)
            {
                if (serviceRegistryConfig.ServiceDefinitions != null)
                    foreach (var serviceDefinition in serviceRegistryConfig.ServiceDefinitions)
                    {
                        if(string.IsNullOrEmpty(serviceDefinition.Id)) throw new MHMServiceRegistryException("");

                        m_serviceTypeToDefinitionIdCache.Add(TypeUtils.GetType(serviceDefinition.InterfaceType), serviceDefinition.Id);
                        m_serviceTypeToServiceDefinitionMap.Add(TypeUtils.GetType(serviceDefinition.InterfaceType), serviceDefinition);
                    }

                if (serviceRegistryConfig.ServiceInstances != null)
                    foreach (var serviceInstanceInfo in serviceRegistryConfig.ServiceInstances)
                    {
                        if (string.IsNullOrEmpty(serviceInstanceInfo.ServiceDefinition) == false && 
                            m_serviceInstanceCache.ContainsKey(serviceInstanceInfo.ServiceDefinition) == false)
                        {
                            m_serviceInstanceCache.Add(serviceInstanceInfo.ServiceDefinition, new List<ServiceInstanceInfo>());
                        }

                        if (serviceInstanceInfo.ServiceDefinition != null)
                            m_serviceInstanceCache[serviceInstanceInfo.ServiceDefinition].Add(serviceInstanceInfo);
                    }
            }
        }

        public ServiceInstanceInfo GetServiceInfo<TService>()
        {
            //lookup the TService interface in the service instance cache to get the ID
            string serviceId = string.Empty;
            if (m_serviceTypeToDefinitionIdCache.TryGetValue(typeof (TService), out serviceId) &&
                m_serviceInstanceCache.ContainsKey(serviceId))
            {
                //make sure that there is more than 1 service instance, return the first instance in the list
                //here we can use a Strategy pattern to determine how we pick which item of the list to return - but for now we will keep it really simple!
                if (m_serviceInstanceCache[serviceId].Count == 0)
                {
                    throw new MHMServiceRegistryException(string.Format("No service instances exist for type {0}", typeof (TService).FullName));
                }

                return m_serviceInstanceCache[serviceId][0];
            }

            throw new MHMServiceRegistryException(string.Format("No service instances exist for type {0}", typeof(TService).FullName));
        }
    }
}

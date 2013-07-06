using System.Collections.Generic;
using MHM.WinFlexOne.Interfaces.Public.ClaimsService;
using MHM.WinFlexOne.Services.Integration.Schema;
using NUnit.Framework;

namespace MHM.WinFlexOne.Services.Integration.Tests
{
    [TestFixture]
    public class ServiceBrokerTests
    {
        [Test]
        public void Locate_Test()
        {
            ServiceRegistryConfig registryConfig = BuildRegsitryConfig();
            IServiceRegistry serviceRegistry = new ConfigBasedServiceRegistry(registryConfig);
            ServiceBrokerConfig config = BuildBrokerConfig();
            IServiceBroker serviceBroker = new ServiceBroker(serviceRegistry, config);
            ITestService testService = serviceBroker.Locate<ITestService>();

            Assert.IsNotNull(testService);
        }

        [Test]
        [ExpectedException(typeof(MHMServiceRegistryException), ExpectedMessage = "No service instances exist for type MHM.WinFlexOne.Interfaces.Public.ClaimsService.IClaimsService")]
        public void Locate_Fails_Test()
        {
            ServiceRegistryConfig registryConfig = BuildRegsitryConfig();
            IServiceRegistry serviceRegistry = new ConfigBasedServiceRegistry(registryConfig);
            ServiceBrokerConfig config = BuildBrokerConfig();
            IServiceBroker serviceBroker = new ServiceBroker(serviceRegistry, config);
            IClaimsService testService = serviceBroker.Locate<IClaimsService>();

            Assert.IsNotNull(testService);
        }

        private ServiceRegistryConfig BuildRegsitryConfig()
        {
            return new ServiceRegistryConfig
                       {
                           ServiceDefinitions = new List<ServiceDefinition>
                                                    {
                                                        new ServiceDefinition
                                                            {
                                                                Id = "TestService",
                                                                DefaultInstance = "TestService",
                                                                InterfaceType = "MHM.WinFlexOne.Services.Integration.Tests.ITestService, MHM.WinFlexOne.Services.Integration.Tests",
                                                                ServiceCreationModel = ServiceCreationEnum.Singleton
                                                            },
                                                    }.ToArray(),
                           ServiceInstances = new List<ServiceInstanceInfo>
                                                  {
                                                      new ObjectServiceInstanceInfo
                                                          {
                                                              Id = "InProcessTestService",
                                                              ServiceDefinition = "TestService",
                                                              ServiceClassName = "MHM.WinFlexOne.Services.Integration.Tests.TestService, MHM.WinFlexOne.Services.Integration.Tests",
                                                          }
                                                  }.ToArray(),
                       };
        }

        private ServiceBrokerConfig BuildBrokerConfig()
        {
            return new ServiceBrokerConfig
                {
                    Container = new ContainerInfo
                                    {
                                        ID = "DynamicProxy",
                                        Type = "MHM.WinFlexOne.Services.Integration.Containers.ProxyContainer, MHM.WinFlexOne.Services.Integration"
                                    },
                    FactoryConfig = new FactoryConfiguration
                                    {
                                        Factories = new List<FactoryInfo>
                                                        {
                                                            new FactoryInfo
                                                                {
                                                                    ID = "InProcessFactory", 
                                                                    Type = "MHM.WinFlexOne.Services.Integration.Factories.ObjectServiceFactory, MHM.WinFlexOne.Services.Integration"
                                                                },
                                                            new FactoryInfo
                                                                {
                                                                    ID = "WcfProxyFactory",
                                                                    Type = "MHM.WinFlexOne.Services.Integration.Factories.ServiceProxyFactory, MHM.WinFlexOne.Services.Integration"
                                                                }
                                                        }.ToArray(),
                                        FactoryOverrides = new List<FactoryOverrideConfig>
                                                                {
                                                                    new FactoryOverrideConfig
                                                                        {
                                                                            ServiceInterface = "MHM.WinFlexOne.Services.Integration.Tests.ITestService, MHM.WinFlexOne.Services.Integration.Tests",
                                                                            Factory = new FactoryReference
                                                                                            {
                                                                                                href = "WcfProxyFactory"
                                                                                            },
                                                                        }
                                                                }.ToArray(),
                                        DefaultFactory = new FactoryReference
                                                                {
                                                                    href = "InProcessFactory"
                                                                },
                                    }
                };
        }
    }
}

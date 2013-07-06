using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHM.WinFlexOne.Interfaces.Public.ClaimsService;
using MHM.WinFlexOne.Services.Integration.Schema;
using NUnit.Framework;

namespace MHM.WinFlexOne.Services.Integration.Tests
{
    [TestFixture]
    public class ServiceRegistryTests
    {
        [Test]
        public void GetServiceInfo_Test()
        {
            ServiceRegistryConfig config = BuildRegsitryConfig();
            IServiceRegistry registry = new ConfigBasedServiceRegistry(config);
            var serviceInfo = registry.GetServiceInfo<ITestService>();

            Assert.IsNotNull(serviceInfo);

            //since the ServiceRegistry is caching the config, if we ask for this service again, we should get back a reference to the same instance
            Assert.AreSame(serviceInfo, registry.GetServiceInfo<ITestService>());
        }

        [Test]
        [ExpectedException(typeof(MHMServiceRegistryException), ExpectedMessage = "No service instances exist for type MHM.WinFlexOne.Interfaces.Public.ClaimsService.IClaimsService")]
        public void GetServiceInfo_ServiceNotRegistered_Test_()
        {
            ServiceRegistryConfig config = BuildRegsitryConfig();
            IServiceRegistry registry = new ConfigBasedServiceRegistry(config);
            var serviceInfo = registry.GetServiceInfo<IClaimsService>();
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
    }
}

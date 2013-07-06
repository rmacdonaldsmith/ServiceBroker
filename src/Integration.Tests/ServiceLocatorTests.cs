using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using MHM.WinFlexOne.Interfaces.Public.CardRequestBatch;
using MHM.WinFlexOne.Services.Integration.Schema;
using MHM.WinFlexOne.Services.ServiceInterfaces;
using NUnit.Framework;
using MHM.WinFlexOne.Services.Integration;
using MHM.WinFlexOne.Interfaces.Public.ImporterService;
using MHM.WinFlexOne.Services.ImporterService;
using MHM.WinFlexOne.Interfaces.Public.ClaimsService;
using Rhino.Mocks;

namespace MHM.WinFlexOne.Services.Integration.Tests
{
    [TestFixture]
    public class ServiceLocatorTests
    {
        [Test]
        public void LocateInProcess_Test()
        {
            IImporterService _ServiceInsance = ServiceLocator.Locate<IImporterService>();

            Assert.IsAssignableFrom(typeof(ImporterService.ImporterService), _ServiceInsance);

            IImporterService _ServiceInsance2 = ServiceLocator.Locate<IImporterService>();

            //make sure they are both refering to the same instance
            Assert.AreSame(_ServiceInsance, _ServiceInsance2);
        }

        [Test]
        public void LocateProxy_Test()
        {
            IClaimsService _ClaimsProxy = ServiceLocator.Locate<IClaimsService>(BindingEnum.NetTcpStreaming);
            Assert.IsAssignableFrom(typeof(IClaimsService), _ClaimsProxy);

            IImporterService _ImporterProxy = ServiceLocator.Locate<IImporterService>(BindingEnum.NetTcp);
            Assert.IsAssignableFrom(typeof(IImporterService), _ImporterProxy);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void LocateFails_Test()
        {
            ITestServiceInterface _ServiceInsance = ServiceLocator.Locate<ITestServiceInterface>();
        }

        [Test]
        public void IServiceLocator_Test()
        {
            ITestServiceInterface serviceInstance = MockRepository.GenerateStub<ITestServiceInterface>();
            IServiceBroker serviceBroker = MockRepository.GenerateStub<IServiceBroker>();
            serviceBroker.Stub(broker => broker.Locate<ITestServiceInterface>())
                .Return(serviceInstance);
            IServiceLocator serviceLocator = new ServiceLocator(serviceBroker);

            var locatedServiceInstance = serviceLocator.Locate<ITestServiceInterface>();

            Assert.IsNotNull(locatedServiceInstance);

            ICardRequestBatchService electionService = MockRepository.GenerateStub<ICardRequestBatchService>();
            serviceBroker.Stub(broker => broker.Locate<ICardRequestBatchService>())
                .Return(new CardRequestBatchService.CardRequestBatchService());

            ICardRequestBatchService locatedElectionService = serviceLocator.Locate<ICardRequestBatchService>();

            Assert.IsNotNull(locatedElectionService);
        }
    }
}

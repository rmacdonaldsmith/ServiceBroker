using Castle.Windsor;
using ServiceBroker.Common;

namespace MHM.WinFlexOne.Services.Integration.Factories
{
    public class ServiceProxyFactory : IServiceFactory
    {
        private IWindsorContainer m_WindsorContainer;

        public ServiceProxyFactory(IWindsorContainer iocContainer)
        {
            Ensure.ParameterIsNotNull(iocContainer, "iocContainer");

            m_WindsorContainer = iocContainer;
        }

        public TService CreateService<TService>(Schema.ServiceInstanceInfo serviceInstanceInfo) where TService : class
        {
            //check if we have configured this service

            return m_WindsorContainer.Resolve<TService>();
        }
    }
}

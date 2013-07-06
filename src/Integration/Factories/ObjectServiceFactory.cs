using MHM.WinFlexOne.Services.Integration.Schema;
using ServiceBroker.Common;

namespace MHM.WinFlexOne.Services.Integration.Factories
{
    public class ObjectServiceFactory : IServiceFactory
    {
        public TService CreateService<TService>(Schema.ServiceInstanceInfo serviceInstanceInfo) where TService : class
        {
            Ensure.ParameterIsNotNull(serviceInstanceInfo, "serviceInstanceInfo");

            var objectServiceInstanceInfo = serviceInstanceInfo as ObjectServiceInstanceInfo;

            if (objectServiceInstanceInfo == null)
            {
                throw new ServiceBrokerException(
                    "ObjectServiceFactory expects an ObjectServiceInstanceInfo parameter but received a " +
                    serviceInstanceInfo.GetType().Name + " parameter");
            }

            //Use TypeUtils to do the Type Work
            return TypeUtils.LoadTypeAndCreateInstance<TService>(
                objectServiceInstanceInfo.ServiceClassName);
        }
    }
}

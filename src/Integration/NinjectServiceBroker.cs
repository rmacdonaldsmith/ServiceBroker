using Ninject;
using ServiceBroker.Common;

namespace MHM.WinFlexOne.Services.Integration
{
    public class NinjectServiceBroker : IServiceBroker
    {
        private readonly IKernel m_ninjectKernel;

        public NinjectServiceBroker(IKernel ninjectKernel)
        {
            Ensure.ParameterIsNotNull(ninjectKernel, "ninjectKernel");

            m_ninjectKernel = ninjectKernel;
        }

        public T Locate<T>() where T : class
        {
            return m_ninjectKernel.Get<T>();
        }
    }
}

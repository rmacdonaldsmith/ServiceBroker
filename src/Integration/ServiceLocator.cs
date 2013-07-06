using System;
using System.Collections.Generic;
using MHM.WinFlexOne.Interfaces.Public.CardRequestBatch;
using MHM.WinFlexOne.Interfaces.Public.ClaimsService;
using MHM.WinFlexOne.Interfaces.Public.ImporterService;
using MHM.WinFlexOne.Services.Integration.Factories;
using ServiceBroker.Common;

namespace MHM.WinFlexOne.Services.Integration
{
    public partial class ServiceLocator : IServiceLocator
    {
        private readonly IServiceBroker m_serviceBroker;

        public ServiceLocator(IServiceBroker serviceBroker)
        {
            Ensure.ParameterIsNotNull(serviceBroker, "serviceBroker");

            m_serviceBroker = serviceBroker;
        }

        TService IServiceLocator.Locate<TService>()
        {
            return m_serviceBroker.Locate<TService>();
        }
    }

    public partial class ServiceLocator
    {
        private static readonly object m_Lock = new object();
        private static bool m_IsInitialized = false;
        private static Dictionary<string, Type> m_SuportedServiceTypeCache = new Dictionary<string, Type>();            //map of    service interface type name :: concrete service type
        private static Dictionary<Type, object> m_InterfaceToInstanceCache = new Dictionary<Type, object>();            //map of    service interface type      :: concrete service instance
        private static Dictionary<Type, object> m_ServiceTypeToInstanceCache = new Dictionary<Type, object>();          //map       concrete service type       :: concrete service instance

        /// <summary>
        /// This overload will create an in-process instance of the service as a default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Locate<T>() where T : class
        {
            return Locate<T>(BindingEnum.InProcess);
        }

        /// <summary>
        /// Locates the service that implements the specified interface and creates a proxy to the service using the binding specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binding"></param>
        /// <returns></returns>
        public static T Locate<T>(BindingEnum binding) where T : class
        {
            if (!m_IsInitialized)
            {
                lock (m_Lock)
                {
                    if (!m_IsInitialized) //check twice because another thread can sneek in here while we are aquiring the lock - never say never!
                    {
                        Init();
                    }
                }
            }

            string _QualifiedServiceName = typeof(T).FullName;

            lock (m_Lock)
            {
                if (!m_SuportedServiceTypeCache.ContainsKey(_QualifiedServiceName))
                {
                    throw new NotSupportedException(string.Format("Service of type '{0}' is not supported.", _QualifiedServiceName));
                }

                if (binding == BindingEnum.InProcess)
                {
                    return BuildInProc<T>(m_SuportedServiceTypeCache[_QualifiedServiceName]);
                }
                else
                {
                    IProxyFactory _Factory = GetProxyFactory<T>(_QualifiedServiceName, binding);
                    return (T)_Factory.Build<T>(binding);
                }
            }
        }

        private static IProxyFactory GetProxyFactory<T>(string qualifiedName, BindingEnum binding)
        {
			if ( qualifiedName.Equals( typeof( IImporterService ).FullName )
				|| qualifiedName.Equals( typeof( IImporterProfileService ).FullName ) )
			{
				return new ImporterProxyFactory();
			}
			if ( qualifiedName.Equals( typeof( IClaimsService ).FullName ) )
			{
				return new ClaimProxyFactory();
			}
			if ( qualifiedName.Equals( typeof( ICardRequestBatchService ).FullName ) )
			{
				return new CardRequestBatchProxyFactory();
			}

			throw new NotSupportedException( string.Format( "Service type '{0}' is not supported.", typeof( T ).Name ) );
        }

        private static T BuildInProc<T>(Type concreteServiceType)
        {
            //first, check if already have an instance of this concrete type - this can happen when one class implements more than one interface (i.e. the ImporterService implements IImporterService and IImporterProfileService)
            //note: this kind of makes our services Singleton by default - we will want to be able to control this through config.
            if (m_ServiceTypeToInstanceCache.ContainsKey(concreteServiceType))
            {
                return (T)m_ServiceTypeToInstanceCache[concreteServiceType];
            }

            //if not, then build a new instance of the concrete type and add to the caches
            if (!m_InterfaceToInstanceCache.ContainsKey(typeof(T)))
            {
                T _ServiceInstance = (T)Activator.CreateInstance(concreteServiceType);
                m_InterfaceToInstanceCache.Add(typeof(T), _ServiceInstance);
                m_ServiceTypeToInstanceCache.Add(concreteServiceType, _ServiceInstance);
            }

            return (T)m_InterfaceToInstanceCache[typeof(T)];
        }

        private static void Init()
        {
            //load our cache with supported service types - in the future this should come from config.
			m_SuportedServiceTypeCache.Add(typeof(IImporterService).FullName, typeof(MHM.WinFlexOne.Services.ImporterService.ImporterService));
            m_SuportedServiceTypeCache.Add(typeof(IClaimsService).FullName, typeof(MHM.WinFlexOne.Services.ClaimsService.ClaimsService));
            m_SuportedServiceTypeCache.Add(typeof(ICardRequestBatchService).FullName, typeof(CardRequestBatchService.CardRequestBatchService));

            m_IsInitialized = true;
        }
    }
}

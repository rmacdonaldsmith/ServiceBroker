using System;
using System.Collections.Generic;
using System.Linq;
using ServiceBroker.Common;
using log4net;
using MHM.WinFlexOne.Services.Integration.Schema;

namespace MHM.WinFlexOne.Services.Integration
{
    public class ServiceBroker : IServiceBroker
    {
        private static ILog Logger = LogManager.GetLogger(typeof(ServiceBroker));
        private readonly IServiceRegistry m_serviceRegistry;
        private readonly IServiceFactory m_defaultServiceFactory;
        private readonly Dictionary<string, Type> m_factoryCache = new Dictionary<string, Type>();       //factoryid             ::  factory type
        private readonly Dictionary<Type, string> m_factoryOverrides = new Dictionary<Type, string>();   //service interface     ::  factoryid
        private readonly Dictionary<string, Type> m_containerCache = new Dictionary<string, Type>();     //containerid           ::  containertype

        //todo: dependency injection to resolve the instance of IServiceRegistry to use here
        public ServiceBroker(IServiceRegistry serviceRegistry, ServiceBrokerConfig serviceBrokerConfig)
        {
            Ensure.ParameterIsNotNull(serviceRegistry, "serviceRegistry");
            Ensure.ParameterIsNotNull(serviceBrokerConfig, "serviceBrokerConfig");

            m_serviceRegistry = serviceRegistry;
            
            //Parse the config
            //load all the factories that have been defined in config
            if (serviceBrokerConfig.FactoryConfig != null)
            {
                foreach (var _Factory in serviceBrokerConfig.FactoryConfig.Factories)
                {
                    Type _FactoryType = TypeUtils.GetType(_Factory.Type);
                    m_factoryCache.Add(_Factory.ID, _FactoryType);
                }

                //set the default factory instance
                if (m_factoryCache.ContainsKey(serviceBrokerConfig.FactoryConfig.DefaultFactory.href) == false)
                {
                    throw new ServiceBrokerException(
                        "Could not find the default factory instance defined in configuration.");
                }

                m_defaultServiceFactory =
                    TypeUtils.CreateInstanceOfType<IServiceFactory>(
                        m_factoryCache[serviceBrokerConfig.FactoryConfig.DefaultFactory.href]);

                //load any factory overrides
                foreach (var _Override in serviceBrokerConfig.FactoryConfig.FactoryOverrides)
                {
                    Type _ServiceInterface = TypeUtils.GetType(_Override.ServiceInterface);
                    m_factoryOverrides.Add(_ServiceInterface, _Override.Factory.href);
                }
            }

            //load the DI container
            if (serviceBrokerConfig.Container != null)
            {
                Type _ContainerType = Type.GetType(serviceBrokerConfig.Container.Type);
                m_containerCache.Add(serviceBrokerConfig.Container.ID, _ContainerType);
            }
        }

        public T Locate<T>() where T : class
        {
            var _ServiceDefinition = m_serviceRegistry.GetServiceInfo<T>();

            //check for any factory overrides for T
            var _FactoryId = m_factoryOverrides.Where(f => f.Key.GetType() == typeof(T)).Select(f => f.Value).SingleOrDefault();
            IServiceFactory _Factory = null;

            if (string.IsNullOrEmpty(_FactoryId))
            {
                _Factory = m_defaultServiceFactory;
            }
            else
            {
                Type _FactoryType = m_factoryCache[_FactoryId];
                _Factory = TypeUtils.CreateInstanceOfType<IServiceFactory>(_FactoryType);
            }

            return _Factory.CreateService<T>(_ServiceDefinition);
        }


        private void Init()
        {


        }
    }
}

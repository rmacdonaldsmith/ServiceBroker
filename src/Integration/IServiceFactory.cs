using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHM.WinFlexOne.Services.Integration.Schema;

namespace MHM.WinFlexOne.Services.Integration
{
    /// <summary>
    /// Represents a factory for building Services using a specific mechanism. Service Factories
    /// are concerned with creational aspects of the service broker, and should internalise or 'represent'
    /// functions such as caching, routing, service aggregation etc, as the ServiceBroker itself should
    /// not be concerned with such things.
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Factories should return (by whatever means) a service implementation (or wrapper) for the 
        /// interface type specified in the TService type parameter
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceInstanceInfo"> </param>
        /// <returns>An instance of TService </returns>
        TService CreateService<TService>(ServiceInstanceInfo serviceInstanceInfo) where TService : class;
    }
}

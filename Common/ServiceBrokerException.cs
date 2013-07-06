using System;
using System.Runtime.Serialization;

namespace ServiceBroker.Common
{
    public class ServiceBrokerException : Exception
    {
                /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBrokerException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ServiceBrokerException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBrokerException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceBrokerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBrokerException"/> class.
        /// </summary>
        public ServiceBrokerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBrokerException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        public ServiceBrokerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

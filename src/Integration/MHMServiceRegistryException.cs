using System;
using System.Runtime.Serialization;

namespace MHM.WinFlexOne.Services.Integration
{
    public class MHMServiceRegistryException : Exception
    {
        public MHMServiceRegistryException()
        {
        }

        public MHMServiceRegistryException(string message) 
            : base(message)
        {
        }

        public MHMServiceRegistryException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected MHMServiceRegistryException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHM.WinFlexOne.Services.Integration
{
    public interface IServiceBroker
    {
        T Locate<T>() where T : class;
    }
}

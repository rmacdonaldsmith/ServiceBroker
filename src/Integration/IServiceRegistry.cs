using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHM.WinFlexOne.Services.Integration.Schema;

namespace MHM.WinFlexOne.Services.Integration
{
    public interface IServiceRegistry
    {
        ServiceInstanceInfo GetServiceInfo<TService>();
    }
}

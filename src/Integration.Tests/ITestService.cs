using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHM.WinFlexOne.Services.Integration.Tests
{
    public interface ITestService
    {
        void DoSomething(Action<string> actionToPerform);
    }
}

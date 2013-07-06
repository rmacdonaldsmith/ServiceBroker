using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHM.WinFlexOne.Services.Integration.Tests
{
    public class TestService : ITestService
    {
        public void DoSomething(Action<string> actionToPerform)
        {
            actionToPerform.Invoke("Hello");
        }
    }
}

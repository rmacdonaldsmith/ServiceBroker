using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHM.Utilities.Context;
using MHM.WinFlexOne.Services.ServiceInterfaces;
using NUnit.Framework;
using Ninject;

namespace MHM.WinFlexOne.Services.Integration.Tests
{
    [TestFixture]
    public class NinjectTests
    {
        [Test]
        public void Test()
        {
            string admCode = "admCode";
            string userName = "robertms";

            IKernel container = GetContainer(admCode, userName);

            
        }

        private IKernel GetContainer(string admCode, string userName)
        {
            IKernel container = new StandardKernel();
            container
                .Bind<IPresenterExecutionContext>()
                .To<ExecutionContext>()
                .InThreadScope()
                .WithPropertyValue("AdministratorCode", admCode)
                .WithPropertyValue("UserName", userName);
                


            return container;
        }
    }
}

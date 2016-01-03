using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace AutoUpdate.Example.ServiceStub
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var service = new Service();
            //ServiceBase.Run(service);
            service.ManualStart();
        }
    }
}

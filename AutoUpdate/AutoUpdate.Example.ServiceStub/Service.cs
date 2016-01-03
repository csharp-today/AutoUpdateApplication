using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace AutoUpdate.Example.ServiceStub
{
    public partial class Service : ServiceBase
    {
        private readonly AutoUpdateServiceRunner _runner;

        public Service()
        {
            InitializeComponent();

            var blob = ConfigurationManager.AppSettings["BlobUrl"];
            _runner = new AutoUpdateServiceRunner(blob);
        }

        public void ManualStart()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            _runner.StartService();
        }

        protected override void OnStop()
        {
            _runner.StopService();
        }
    }
}

using AutoUpdate.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUpdate
{
    public class AutoUpdateServiceRunner : BaseRunner
    {
        private IAutoUpdateService _service;

        private IAutoUpdateService Service
        {
            get
            {
                if (_service == null)
                {
                    _service = FindInterface<IAutoUpdateService>();
                }
                return _service;
            }
        }

        public AutoUpdateServiceRunner(string blobUrl) : base(blobUrl) { }

        public void StartService()
        {
            Service.Start();
        }

        public void StopService()
        {
            Service.Stop();
            WaitForFirstUpdateTrail();
        }
    }
}

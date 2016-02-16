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
            try
            {
                Service.Start();
            }
            catch (Exception ex)
            {
                WaitForFirstUpdateTrail();
                throw new Exception("Can't start service", ex);
            }
        }

        public void StopService()
        {
            try
            {
                Service.Stop();
            }
            finally
            {
                WaitForFirstUpdateTrail();
            }
        }
    }
}

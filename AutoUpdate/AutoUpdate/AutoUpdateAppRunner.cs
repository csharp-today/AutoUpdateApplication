using AutoUpdate.Blob;
using AutoUpdate.Repo;
using AutoUpdate.Updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdate
{
    public class AutoUpdateAppRunner
    {
        public AutoUpdateAppRunner(string blobUrl)
        {
            var appRepo = new ApplicationRepository();
            var appUpdater = new ApplicationUpdater(appRepo, blobUrl);
            appUpdater.Run();
        }

        public void Run(string[] args)
        {
            System.Threading.Thread.Sleep(-1);
        }
    }
}

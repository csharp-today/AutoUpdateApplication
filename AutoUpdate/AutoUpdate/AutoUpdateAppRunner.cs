using AutoUpdate.Blob;
using AutoUpdate.Common;
using AutoUpdate.Repo;
using AutoUpdate.Updater;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace AutoUpdate
{
    public class AutoUpdateAppRunner
    {
        private readonly ApplicationUpdater _appUpdater;
        private readonly ApplicationRepository _repo;

        public AutoUpdateAppRunner(string blobUrl)
        {
            _repo = new ApplicationRepository();
            _appUpdater = new ApplicationUpdater(_repo, blobUrl);
            _appUpdater.Run();
        }

        public void Run(string[] args)
        {
            WaitForAppAvailability();

            try
            {
                var dir = _repo.GenerateExecutiveArea();
                var type = FindExecutive(dir);

                var app = (IAutoUpdateApplication)Activator.CreateInstance(type);
                app.Start(args);
            }
            catch (Exception) when (InvalidateRepo()) { }

            WaitForFirstUpdateTrail();
        }

        private Type FindExecutive(string dir)
        {
            var apps = new List<Type>();
            var libs = Directory.GetFiles(dir, "*.dll");
            foreach(var lib in libs)
            {
                var types = InspectLibraryForExecutable(lib);
                apps.AddRange(types);
            }

            return apps[0];
        }

        private bool InvalidateRepo()
        {
            _repo.Clear();
            return false;
        }

        private IEnumerable<Type> InspectLibraryForExecutable(string path)
        {
            var apps = new List<Type>();
            var asm = Assembly.LoadFile(path);
            foreach (var type in asm.GetTypes())
            {
                if (!type.IsInterface)
                {
                    var interfaces = type.GetInterfaces();
                    Type autoUpdateInterface = null;
                    foreach (var i in interfaces)
                    {
                        if (i.Name == "IAutoUpdateApplication")
                        {
                            autoUpdateInterface = i;
                        }
                    }

                    if (autoUpdateInterface != null)
                    {
                        apps.Add(type);
                    }
                }
            }

            return apps;
        }

        private void WaitForAppAvailability()
        {
            while (!_repo.HasData)
            {
                WaitSleep();
            }
        }

        private void WaitForFirstUpdateTrail()
        {
            while (!_appUpdater.FirstUpdateTried)
            {
                WaitSleep();
            }
        }

        private static void WaitSleep()
        {
            Thread.Sleep(100);
        }
    }
}

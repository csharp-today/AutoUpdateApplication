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
    public class BaseRunner
    {
        private readonly ApplicationUpdater _appUpdater;
        private readonly ApplicationRepository _repo;

        public BaseRunner(string blobUrl)
        {
            _repo = new ApplicationRepository();
            _appUpdater = new ApplicationUpdater(_repo, blobUrl);
            _appUpdater.Run();
        }

        protected T FindInterface<T>()
        {
            WaitForAppAvailability();

            try
            {
                var dir = _repo.GenerateExecutiveArea();
                var type = FindExecutive(dir, typeof(T).Name);

                var app = (T)Activator.CreateInstance(type);
                return app;
            }
            catch (Exception) when (InvalidateRepo())
            {
                throw new Exception();
            }
        }

        protected void WaitForFirstUpdateTrail()
        {
            while (!_appUpdater.FirstUpdateTried)
            {
                WaitSleep();
            }
        }

        private Type FindExecutive(string dir, string interfaceName)
        {
            var apps = new List<Type>();
            var libs = Directory.GetFiles(dir, "*.dll");
            foreach (var lib in libs)
            {
                var types = InspectLibraryForExecutable(lib, interfaceName);
                apps.AddRange(types);
            }

            return apps[0];
        }

        private bool InvalidateRepo()
        {
            _repo.Clear();
            return false;
        }

        private IEnumerable<Type> InspectLibraryForExecutable(string path, string interfaceName)
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
                        if (i.Name == interfaceName)
                        {
                            autoUpdateInterface = i;
                            break;
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

        private static void WaitSleep()
        {
            Thread.Sleep(100);
        }
    }
}

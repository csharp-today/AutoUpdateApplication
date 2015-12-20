using AutoUpdate.Blob;
using AutoUpdate.Repo;
using AutoUpdate.Updater;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoUpdate
{
    public class AutoUpdateAppRunner
    {
        private readonly ApplicationRepository _repo;

        public AutoUpdateAppRunner(string blobUrl)
        {
            _repo = new ApplicationRepository();
            var appUpdater = new ApplicationUpdater(_repo, blobUrl);
            appUpdater.Run();
        }

        public void Run(string[] args)
        {
            WaitForAppAvailability();
            var dir = _repo.GenerateExecutiveArea();
            var type = FindExecutive(dir);

            var app = (IAutoUpdateApplication)Activator.CreateInstance(type);
            app.Start(args);
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

            return apps.First();
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
                    var autoUpdateInterfaces =
                        interfaces.Where(i => i.Name == "IAutoUpdateApplication");
                    if (autoUpdateInterfaces.Count() > 0)
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
                Thread.Sleep(100);
            }
        }
    }
}

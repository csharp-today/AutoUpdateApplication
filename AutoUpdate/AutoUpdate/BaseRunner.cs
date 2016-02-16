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

        private string _executiveAreaDirectory;

        private string ExecutiveAreaDirectory
        {
            get
            {
                if (_executiveAreaDirectory == null)
                {
                    _executiveAreaDirectory = _repo.GenerateExecutiveArea();
                }
                return _executiveAreaDirectory;
            }
        }

        public BaseRunner(string blobUrl)
        {
            _repo = new ApplicationRepository();
            _appUpdater = new ApplicationUpdater(_repo, blobUrl);
            _appUpdater.Run();

            AppDomain.CurrentDomain.AssemblyResolve += Domain_AssemblyResolve;
        }

        protected T FindInterface<T>()
        {
            WaitForAppAvailability();

            try
            {
                var dir = ExecutiveAreaDirectory;
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

        private Assembly Domain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var dir = _executiveAreaDirectory;
            if (string.IsNullOrEmpty(dir))
            {
                return null;
            }

            var asmName = new AssemblyName(args.Name);
            var files = Directory.GetFiles(dir, asmName.Name + ".dll");
            if (files == null || files.Length == 0)
            {
                files = Directory.GetFiles(dir, asmName.Name + ".exe");
            }

            if (files == null || files.Length == 0)
            {
                return null;
            }

            var asm = Assembly.LoadFile(files[0]);
            return asm;
        }

        private Type FindExecutive(string dir, string interfaceName)
        {
            var libsDll = Directory.GetFiles(dir, "*.dll");
            var libsExe = Directory.GetFiles(dir, "*.exe");

            var libs = new string[libsDll.Length + libsExe.Length];
            libsDll.CopyTo(libs, 0);
            libsExe.CopyTo(libs, libsDll.Length);

            var apps = new List<Type>();
            foreach (var lib in libs)
            {
                try
                {
                    var types = InspectLibraryForExecutable(lib, interfaceName);
                    apps.AddRange(types);
                }
                catch (Exception) { }
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

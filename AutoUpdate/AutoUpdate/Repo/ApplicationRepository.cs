using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoUpdate.Repo
{
    internal class ApplicationRepository
    {
        private readonly string _dataPath;
        private readonly object _locker = new object();

        public DateTime ApplicationTimeStamp
        {
            get
            {
                lock (_locker)
                {
                    return Settings.Default.RepoTime;
                }
            }
        }

        public bool HasData
        {
            get
            {
                lock (_locker)
                {
                    ExistenceCheck();
                    return Settings.Default.HasData;
                }
            }
        }

        private Guid AppId
        {
            get
            {
                var guid = Settings.Default.AppId;
                if (guid == Guid.Empty)
                {
                    guid = Guid.NewGuid();
                    Settings.Default.AppId = guid;
                    Settings.Default.Save();
                }
                return guid;
            }
        }

        public ApplicationRepository()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appPath = Path.Combine("AutoUpdate", AppId.ToString());
            _dataPath = Path.Combine(appDataPath, appPath);
        }

        public void AddFile(string name, MemoryStream data)
        {
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }

            var path = Path.Combine(_dataPath, name);
            using (var stream = File.Create(path))
            {
                stream.Write(data.GetBuffer(), 0, (int)data.Length);
            }
        }

        public void Clear()
        {
            lock (_locker)
            {
                Settings.Default.HasData = false;
                Settings.Default.RepoTime = DateTime.MinValue;
                Settings.Default.Save();

                if (Directory.Exists(_dataPath))
                {
                    Directory.Delete(_dataPath, true);
                }
            }
        }

        public string GenerateExecutiveArea()
        {
            lock (_locker)
            {
                var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var files = Directory.GetFiles(_dataPath);
                foreach(var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var targetPath = Path.Combine(dir, fileName);
                    File.Copy(file, targetPath);
                }

                return dir;
            }
        }

        public void SetHadData(DateTime timeStamp)
        {
            lock (_locker)
            {
                Settings.Default.RepoTime = timeStamp;
                Settings.Default.HasData = true;
                Settings.Default.Save();
            }
        }

        private void ExistenceCheck()
        {
            if (!Directory.Exists(_dataPath))
            {
                Settings.Default.HasData = false;
                Settings.Default.RepoTime = DateTime.MinValue;
                Settings.Default.Save();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdate.Repo
{
    internal class ApplicationRepository
    {
        private readonly string _dataPath;

        public DateTime ApplicationTimeStamp
        {
            get
            {
                return Settings.Default.RepoTime;
            }
        }

        public ApplicationRepository()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _dataPath = Path.Combine(appData, "AutoUpdate");
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
                data.CopyTo(stream);
            }
        }

        public void Clear()
        {
            Settings.Default.HasData = false;
            Settings.Default.RepoTime = DateTime.MinValue;
            Settings.Default.Save();

            if (Directory.Exists(_dataPath))
            {
                Directory.Delete(_dataPath, true);
            }
        }

        public void SetHadData(DateTime timeStamp)
        {
            Settings.Default.RepoTime = timeStamp;
            Settings.Default.HasData = true;
            Settings.Default.Save();
        }
    }
}

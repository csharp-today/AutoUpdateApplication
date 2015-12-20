using AutoUpdate.Blob;
using AutoUpdate.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoUpdate.Updater
{
    internal class ApplicationUpdater
    {
        private const int ConsecutiveInterval = 15 * 60 * 1000; // 15 min

        private readonly BlobReader _blobReader;
        private int _interval = 60 * 1000; // 1 min
        private ApplicationRepository _repo;
        private Task _task;

        public ApplicationUpdater(ApplicationRepository repo, string blobUrl)
        {
            _blobReader = new BlobReader(blobUrl);
            _repo = repo;
        }

        public void Run()
        {
            if (_task == null)
            {
                _task = Task.Run(() => CheckUpdates());
            }
        }

        private void CheckUpdates()
        {
            while (true)
            {
                GetUpdate();
                Thread.Sleep(_interval);
            }
        }

        private void GetUpdate()
        {
            try
            {
                GetUpdateUnsafe();
                _interval = ConsecutiveInterval;
            }
            catch (Exception) { }
        }

        private void GetUpdateUnsafe()
        {
            if (_repo.ApplicationTimeStamp < _blobReader.UpdateTime)
            {
                _blobReader.DownloadFiles();
                _repo.Clear();

                foreach(var file in _blobReader.Files)
                {
                    _repo.AddFile(file.Name, file.Data);
                }

                _repo.SetHadData(_blobReader.UpdateTime);
            }
        }
    }
}

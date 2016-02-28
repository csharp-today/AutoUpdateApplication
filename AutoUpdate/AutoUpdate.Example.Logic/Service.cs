using AutoUpdate.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoUpdate.Example.Logic
{
    public class Service : IAutoUpdateService
    {
        private const string LogFileName = "AutoUpdateServiceExample.log";
        private string LogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            LogFileName);

        public void Initialize(IAutoUpdateMetadata metadata)
        {
        }

        public void Start()
        {
            Write("AutoUpdate.Example.Logic - START");
        }

        public void Stop()
        {
            Write("AutoUpdate.Example.Logic - STOP");
        }

        private void Write(string message)
        {
            using (var writer = File.AppendText(LogPath))
            {
                writer.WriteLine(message);
            }
        }
    }
}

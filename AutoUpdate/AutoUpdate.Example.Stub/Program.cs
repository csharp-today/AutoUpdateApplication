using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace AutoUpdate.Example.Stub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var blob = ConfigurationManager.AppSettings["BlobUrl"];
            var app = new AutoUpdateAppRunner(blob);
            app.Run(args);
        }
    }
}

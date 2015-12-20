using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

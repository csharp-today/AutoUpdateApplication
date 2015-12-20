using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdate.Example.Logic
{
    public class Application : IAutoUpdateApplication
    {
        public void Start(string[] args)
        {
            Console.WriteLine("True logic should be here");
        }
    }
}

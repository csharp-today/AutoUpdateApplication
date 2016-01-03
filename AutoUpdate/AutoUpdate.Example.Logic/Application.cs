using AutoUpdate.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUpdate.Example.Logic
{
    public class Application : IAutoUpdateApplication
    {
        public void Start(string[] args)
        {
            Console.WriteLine("True logic should be here");
            Console.WriteLine("I'm using AutoUpdate.Common 1.0.0 NuGet package.");
        }
    }
}

﻿using AutoUpdate.Blob;
using AutoUpdate.Common;
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
    public class AutoUpdateAppRunner : BaseRunner
    {
        public AutoUpdateAppRunner(string blobUrl) : base(blobUrl) { }

        public void Run(string[] args)
        {
            try
            {
                var app = FindInterface<IAutoUpdateApplication>();
                app.Initialize(Metadata);
                app.Start(args);
            }
            finally
            {
                WaitForFirstUpdateTrail();
            }
        }
    }
}

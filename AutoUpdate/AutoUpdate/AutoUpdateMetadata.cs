using AutoUpdate.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUpdate
{
    internal class AutoUpdateMetadata : IAutoUpdateMetadata
    {
        public string BlobSourceUrl { get; set; }
    }
}

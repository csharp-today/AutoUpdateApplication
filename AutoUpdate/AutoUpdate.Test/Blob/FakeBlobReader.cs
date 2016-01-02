using AutoUpdate.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdate.Test.Blob
{
    internal class FakeBlobReader : BlobReader
    {
        public FakeBlobReader() : base(null)
        {
            // Set _metadata so GetMetadata is not called
            _metadata = string.Empty;
        }

        public BlobFile[] RunGetFiles(string metadata)
        {
            return GetFiles(metadata);
        }

        public DateTime RunGetUpdateTime(IEnumerable<BlobFile> files)
        {
            return GetUpdateTime(files);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace AutoUpdate.Blob
{
    internal class BlobReader
    {
        private BlobFile[] _files;
        private string _metadata;
        private DateTime? _updateTime;
        private readonly string _url;

        public IEnumerable<BlobFile> Files
        {
            get
            {
                if (_files == null)
                {
                    _files = GetFiles();
                }
                return _files;
            }
        }

        public string Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = GetMetadata();
                }
                return _metadata;
            }
        }

        public DateTime UpdateTime
        {
            get
            {
                if (_updateTime == null)
                {
                    _updateTime = GetUpdateTime();
                }
                return _updateTime.Value;
            }
        }

        public BlobReader(string url)
        {
            _url = url;
        }

        public void DownloadFiles()
        {
            foreach(var file in Files)
            {
                file.Download();
            }
        }

        private BlobFile[] GetFiles()
        {
            var metadataXml = Metadata;
            XDocument doc = XDocument.Parse(metadataXml);
            var fileNodes = doc.Descendants(XName.Get("Blob"));
            var fileXmls = fileNodes.Select(n => n.ToString());
            var files = fileXmls.Select(xml => new BlobFile(xml));
            return files.ToArray();
        }

        private string GetMetadata()
        {
            var request = WebRequest.Create(
                _url + "?restype=container&comp=list");
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var metadata = reader.ReadToEnd();
                return metadata;
            }
        }

        private DateTime GetUpdateTime()
        {
            var updateTimes = Files.Select(f => f.UpdateTime);
            var latest = updateTimes.Max();
            return latest;
        }
    }
}

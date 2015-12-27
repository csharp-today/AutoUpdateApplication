using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

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
            var doc = new XmlDocument();
            doc.LoadXml(metadataXml);
            var fileNodes = doc.SelectNodes("//Blob");

            var files = new List<BlobFile>(fileNodes.Count);
            foreach(XmlNode fileNode in fileNodes)
            {
                var xml = fileNode.OuterXml;
                var blobFile = new BlobFile(xml);
                files.Add(blobFile);
            }

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
            DateTime latest = DateTime.MinValue;
            foreach (var file in Files)
            {
                if (file.UpdateTime > latest)
                {
                    latest = file.UpdateTime;
                }
            }
            return latest;
        }
    }
}

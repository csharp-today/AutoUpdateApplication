using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace AutoUpdate.Blob
{
    public class BlobReader
    {
        private BlobFile[] _files;
        protected string _metadata;
        private DateTime? _updateTime;
        private readonly string _url;

        public IEnumerable<BlobFile> Files
        {
            get
            {
                if (_files == null)
                {
                    _files = GetFiles(Metadata);
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
                    _updateTime = GetUpdateTime(Files);
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

        protected BlobFile[] GetFiles(string metadataXml)
        {
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

        protected DateTime GetUpdateTime(IEnumerable<BlobFile> files)
        {
            DateTime latest = DateTime.MinValue;
            foreach (var file in files)
            {
                if (file.UpdateTime > latest)
                {
                    latest = file.UpdateTime;
                }
            }
            return latest;
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
    }
}

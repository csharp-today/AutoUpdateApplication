using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoUpdate.Blob
{
    internal class BlobFile
    {
        private readonly XDocument xml;

        private MemoryStream _cache;

        public MemoryStream Data { get { return _cache; } }

        public string Name
        {
            get
            {
                var nameNode = xml.Descendants(XName.Get("Name")).First();
                return nameNode.Value;
            }
        }

        public DateTime UpdateTime
        {
            get
            {
                var timeNode = xml.Descendants(XName.Get("Last-Modified")).First();
                var time = Convert.ToDateTime(timeNode.Value);
                return time;
            }
        }

        public string Url
        {
            get
            {
                var urlNode = xml.Descendants(XName.Get("Url")).First();
                return urlNode.Value;
            }
        }

        public BlobFile(string metadataXml)
        {
            xml = XDocument.Parse(metadataXml);
        }

        public void Download()
        {
            var request = WebRequest.Create(Url);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                _cache = new MemoryStream();
                stream.CopyTo(_cache);
                _cache.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}

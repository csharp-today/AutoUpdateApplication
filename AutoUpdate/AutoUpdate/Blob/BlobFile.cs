using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace AutoUpdate.Blob
{
    internal class BlobFile
    {
        private readonly XmlDocument xml;

        private MemoryStream _cache;

        public MemoryStream Data { get { return _cache; } }

        public string Name
        {
            get
            {
                var nameNode = xml.SelectSingleNode("//Name");
                return nameNode.InnerText;
            }
        }

        public DateTime UpdateTime
        {
            get
            {
                var timeNode = xml.SelectSingleNode("//Last-Modified");
                var time = Convert.ToDateTime(timeNode.InnerText);
                return time;
            }
        }

        public string Url
        {
            get
            {
                var urlNode = xml.SelectSingleNode("//Url");
                return urlNode.InnerText;
            }
        }

        public BlobFile(string metadataXml)
        {
            xml = new XmlDocument();
            xml.LoadXml(metadataXml);
        }

        public void Download()
        {
            var request = WebRequest.Create(Url);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                _cache = new MemoryStream();

                const int BufferSize = 8 * 1024;
                byte[] buffer = new byte[BufferSize];
                int len = 0;
                while ((len = stream.Read(buffer, 0, BufferSize)) != 0)
                {
                    _cache.Write(buffer, 0, len);
                }

                _cache.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}

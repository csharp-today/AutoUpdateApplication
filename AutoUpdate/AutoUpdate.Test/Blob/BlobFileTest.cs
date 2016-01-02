using AutoUpdate.Blob;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdate.Test.Blob
{
    [TestClass]
    public class BlobFileTest
    {
        private const string Xml = @"<Blob>
	<Name>{0}</Name>
	<Url>{1}</Url>
	<Properties>
		<Last-Modified>{2}</Last-Modified>
		<Etag>0x8D30E95180DF18D</Etag>
		<Content-Length>4096</Content-Length>
		<Content-Type>application/octet-stream</Content-Type>
		<Content-Encoding/>
		<Content-Language/>
		<Content-MD5>/I7z20BcpWt2KQgBYlP1qA==</Content-MD5>
		<Cache-Control/>
		<BlobType>BlockBlob</BlobType>
		<LeaseStatus>unlocked</LeaseStatus>
	</Properties>
</Blob>
";

        [TestMethod]
        public void ReturnName()
        {
            // Arrange
            const string Name = "BlobFileName.txt";
            var xml = GenerateMetadata(name: Name);
            var blobFile = new BlobFile(xml);

            // Act
            var name = blobFile.Name;

            // Assert
            Assert.AreEqual(Name, name);
        }

        [TestMethod]
        public void ReturnUpdateTime()
        {
            //Arrange
            var date = DateTime.Now.ToUniversalTime();
            BlobFile blobFile = GenerateBlobFile(date);

            // Act
            var updateTime = blobFile.UpdateTime;

            // Assert
            var diff = date - updateTime;
            Assert.IsTrue(diff.TotalSeconds < 1);
            Assert.IsTrue(updateTime.Kind == DateTimeKind.Utc);
        }

        [TestMethod]
        public void ReturnUrl()
        {
            // Arrange
            const string Url = @"https://awesome.url/";
            var xml = GenerateMetadata(url: Url);
            var blobFile = new BlobFile(xml);

            // Act
            var url = blobFile.Url;

            // Assert
            Assert.AreEqual(Url, url);
        }

        public static BlobFile GenerateBlobFile(DateTime universalDateTime)
        {
            var formattedDate = GetBlobTimeString(universalDateTime);
            var xml = GenerateMetadata(date: formattedDate);
            var blobFile = new BlobFile(xml);
            return blobFile;
        }

        private static string GenerateMetadata(string name = "NAME", string url = "URL", string date = "Sun, 1 Dec 2000 00:00:01 GMT")
        {
            var xml = string.Format(Xml, name, url, date);
            return xml;
        }

        private static string GetBlobTimeString(DateTime date)
        {
            return date.ToString("ddd, d MMM yyyy HH:mm:ss") + " GMT";
        }
    }
}

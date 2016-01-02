using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdate.Test.Blob
{
    [TestClass]
    public class BlobReaderTest
    {
        [TestMethod]
        public void GetFiles()
        {
            // Arrange
            const string Metadata = @"<EnumerationResults ContainerName=""https://bojkowski.blob.core.windows.net/auto-upload-example"">{0}</EnumerationResults>";
            const string Blobs = @"<Blob/><Blob/><Blob/>";
            var metadata = string.Format(Metadata, Blobs);
            var reader = new FakeBlobReader();

            // Act
            var blobFiles = reader.RunGetFiles(metadata);

            // Assert
            Assert.IsNotNull(blobFiles);
            Assert.AreEqual(3, blobFiles.Length);
        }

        [TestMethod]
        public void UpdateTime()
        {
            // Arrange
            var maxDate = DateTime.Now.ToUniversalTime();
            var blobFiles = new[]
            {
                BlobFileTest.GenerateBlobFile(DateTime.MinValue.ToUniversalTime()),
                BlobFileTest.GenerateBlobFile(maxDate)
            };
            var reader = new FakeBlobReader();

            // Act
            var updateTime = reader.RunGetUpdateTime(blobFiles);

            // Assert
            var diff = maxDate - updateTime;
            Assert.IsTrue(diff.TotalSeconds < 1);
        }
    }
}

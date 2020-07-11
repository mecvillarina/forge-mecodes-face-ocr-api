
using AtlassianForgeFaceDetect.Functions.Abstractions;
using AtlassianForgeFaceDetect.Functions.Services.Abstraction;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AtlassianForgeFaceDetect.Functions.Services
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly IAzureStorageAccountService _azureStorageAccountService;
        public AzureBlobStorageService(IAzureStorageAccountService azureStorageAccountService)
        {
            _azureStorageAccountService = azureStorageAccountService;
        }

        public string GetBlobContainerPath(string containerName)
        {
            var container = _azureStorageAccountService.CreateBlobContainer(containerName);
            return container.Uri.AbsoluteUri;
        }

        public Tuple<bool, string> Upload(Stream stream, string containerName, string filename)
        {
            try
            {
                var container = _azureStorageAccountService.CreateBlobContainer(containerName);

                stream.Seek(0, SeekOrigin.Begin);
                var blockBlob = container.GetBlobClient(filename);

                string contentType = string.Empty;
                var blobHttpHeader = new BlobHttpHeaders();
                string extension = Path.GetExtension(blockBlob.Uri.AbsoluteUri);
                switch (extension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".pdf":
                        contentType = "application/pdf";
                        break;
                    default:
                        contentType = "application/octet-stream";
                        break;
                }

                if (!string.IsNullOrEmpty(contentType))
                {
                    blobHttpHeader.ContentType = contentType;
                }

                var uploadedBlob = blockBlob.Upload(stream, blobHttpHeader);
                return new Tuple<bool, string>(true, contentType);
            }
            catch
            {

            }

            return new Tuple<bool, string>(false, string.Empty);
        }
    }
}

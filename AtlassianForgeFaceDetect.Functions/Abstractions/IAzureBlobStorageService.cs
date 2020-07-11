using System;
using System.IO;
using System.Threading.Tasks;

namespace AtlassianForgeFaceDetect.Functions.Services.Abstraction
{
    public interface IAzureBlobStorageService
    {
        string GetBlobContainerPath(string containerName);
        Tuple<bool, string> Upload(Stream stream, string containerName, string filename);
    }
}
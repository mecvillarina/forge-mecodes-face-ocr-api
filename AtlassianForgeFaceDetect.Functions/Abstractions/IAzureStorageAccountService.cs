using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtlassianForgeFaceDetect.Functions.Abstractions
{
    public interface IAzureStorageAccountService
    {
        string StorageConnectionString { get; }
        Task<CloudTable> CreateTableAsync(string tableName);
        BlobContainerClient CreateBlobContainer(string containerName);
    }
}

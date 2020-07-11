using AtlassianForgeFaceDetect.Functions.Abstractions;
using AtlassianForgeFaceDetect.Functions.Common;
using AtlassianForgeFaceDetect.Functions.Services;
using AtlassianForgeFaceDetect.Functions.Services.Abstraction;
using AtlassianForgeFaceDetect.Functions.Services.Abstractions;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AtlassianForgeFaceDetect.Functions.Startup))]

namespace AtlassianForgeFaceDetect.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IFaceClient, FaceClient>(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();

                string endpoint = configuration[AppSettings.FaceEndpoint];
                string key = configuration[AppSettings.FaceKey];

                return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
            });

            builder.Services.AddSingleton<IAzureStorageAccountService, AzureStorageAccountService>(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                string azureStorage = configuration[AppSettings.AzureWebJobsStorage];
                return new AzureStorageAccountService(azureStorage);
            });

            builder.Services.AddSingleton<IOcrService, OcrService>();
            builder.Services.AddSingleton<IAzureBlobStorageService, AzureBlobStorageService>();
            builder.Services.AddSingleton<IFaceService, FaceService>();
        }
    }
}

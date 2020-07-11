using AtlassianForgeFaceDetect.Functions.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using SixLabors.ImageSharp.Processing;
using System.Linq;
using System.Security.Cryptography;
using AtlassianForgeFaceDetect.Functions.Services.Abstraction;
using AtlassianForgeFaceDetect.Functions.Common;
using System.Collections.Generic;
using AtlassianForgeFaceDetect.Functions.Models;

namespace AtlassianForgeFaceDetect.Functions
{
    public class FaceDetect
    {
        private readonly IConfiguration _configuration;
        private readonly IFaceService _faceService;
        private readonly IAzureBlobStorageService _azureBlobStorageService;
        public FaceDetect(IConfiguration configuration,
            IFaceService faceService,
            IAzureBlobStorageService azureBlobStorageService)
        {
            _configuration = configuration;
            _faceService = faceService;
            _azureBlobStorageService = azureBlobStorageService;
        }

        [FunctionName("FaceDetect")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "face/FaceDetect")] HttpRequest req, ExecutionContext context,
            ILogger log)
        {
            if (req.Query.ContainsKey("imagePath"))
            {
                string imagePath = req.Query["imagePath"];

                try
                {
                    string filename = Guid.NewGuid().ToString() + ".png";
                    var destinationPath = System.IO.Path.Combine(Path.GetTempPath(), $"Images");

                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    destinationPath = $"{destinationPath}\\";
                    string localImagePath = $"{destinationPath}{filename}";

                    WebClient client = new WebClient();
                    client.DownloadFile(imagePath, localImagePath);

                    using (Image image = Image.Load(localImagePath))
                    {
                        double scale = 1;
                        double maxdimen = 1200;

                        if (image.Height > image.Width && image.Height > maxdimen)
                        {
                            scale = maxdimen / (double)image.Height;
                        }
                        else if (image.Width > image.Height && image.Width > maxdimen)
                        {
                            scale = maxdimen / (double)image.Width;
                        }

                        image.Mutate(x => x.Resize(Convert.ToInt32(image.Width * scale), Convert.ToInt32(image.Height * scale)));

                        image.Save(localImagePath);
                    }


                    var data = File.ReadAllBytes(localImagePath);

                    string originalImagePath = string.Empty;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Write(data, 0, data.Length);
                        _azureBlobStorageService.Upload(stream, BlobContainers.OriginalPhotos, filename);
                        originalImagePath = _azureBlobStorageService.GetBlobContainerPath(BlobContainers.OriginalPhotos) + "/" + filename;
                    }

                    var faces = _faceService.GetFaces(originalImagePath);

                    var faceList = new List<Tuple<string, int>>();

                    using (Image image = Image.Load(localImagePath))
                    {
                        string blobContainerPath = _azureBlobStorageService.GetBlobContainerPath(BlobContainers.Faces);

                        for (int i = 0; i < faces.Count; i++)
                        {
                            var face = faces[i];

                            var rectangle = face.FaceRectangle;

                            int padding = 10;
                            int x = rectangle.Left - padding < 0 ? 0 : rectangle.Left - padding;
                            int y = rectangle.Top - padding < 0 ? 0 : rectangle.Top - padding;
                            int width = (rectangle.Left + rectangle.Width) + padding > image.Width ? image.Width : rectangle.Width + padding;
                            int height = (rectangle.Top + rectangle.Height) + padding > image.Height ? image.Height : rectangle.Height + padding;

                            using (var copy = image.Clone(img => img.Crop(new Rectangle(x, y, width, height))))
                            {
                                copy.Mutate(x => x.Resize(100, 100));
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    copy.SaveAsPng(stream);
                                    stream.Seek(0, SeekOrigin.Begin);
                                    string faceFilename = $"{i}_{filename}";
                                    _azureBlobStorageService.Upload(stream, BlobContainers.Faces, faceFilename);
                                    faceList.Add(new Tuple<string, int>($"{blobContainerPath}/{faceFilename}", width));
                                }
                            }
                        }
                    }

                    faceList = faceList.OrderBy(x => x.Item2).ToList();
                    File.Delete(localImagePath);
                    return new OkObjectResult(new FaceDetectResponseModel()
                    {
                        OriginalImage = originalImagePath,
                        Faces = faceList.Select(x => x.Item1).ToList()
                    });
                }
                catch (Exception ex)
                {
                    return new BadRequestErrorMessageResult(ex.InnerException.Message);
                }
            }

            return new NoContentResult();
        }
    }
}

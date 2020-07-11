using AtlassianForgeFaceDetect.Functions.Models;
using AtlassianForgeFaceDetect.Functions.Services;
using AtlassianForgeFaceDetect.Functions.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace AtlassianForgeFaceDetect.Functions
{
    public class ImageToTextFunction
    {
        private readonly IOcrService _ocrService;

        public ImageToTextFunction(IOcrService ocrService)
        {
            _ocrService = ocrService;
        }

        [FunctionName("ImageToTextFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ocr/ReadText")] HttpRequest req, ExecutionContext context, ILogger log)
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

                    var textValue = _ocrService.GetText(data);

                    File.Delete(localImagePath);

                    return new OkObjectResult(new ImageToTextResponseModel()
                    {
                        TextValue = textValue 
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

using AtlassianForgeFaceDetect.Functions.Models;
using AtlassianForgeFaceDetect.Functions.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AtlassianForgeFaceDetect.Functions.Services
{
    public class OcrService : IOcrService
    {
        private readonly IConfiguration _configuration;

        public OcrService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetText(byte[] byteData)
        {
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("ComputerVisionKey"));

                string requestParameters = "language=unk&detectOrientation=true";
                string uri = $"{_configuration["ComputerVisionEndpoint"]}vision/v2.1/ocr" + "?" + requestParameters;

                HttpResponseMessage response;
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");
                    response = client.PostAsync(uri, content).Result;
                }

                // Asynchronously get the JSON response.
                string contentString = response.Content.ReadAsStringAsync().Result;

                // Display the JSON response.
                var result = JsonConvert.DeserializeObject<ReadTextResultModel>(contentString);

                StringBuilder strBuilder = new StringBuilder();

                foreach(var region in result.Regions)
                {
                    foreach(var line in region.Lines)
                    {
                        string lineWords = string.Join(" ", line.Words.Select(x => x.Text));
                        strBuilder.AppendLine(lineWords);        
                    }

                    strBuilder.AppendLine();
                    strBuilder.AppendLine();
                }

                return strBuilder.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }

            return "";
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}

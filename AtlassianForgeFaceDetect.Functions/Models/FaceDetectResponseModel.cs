using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtlassianForgeFaceDetect.Functions.Models
{
    public class FaceDetectResponseModel
    {
        [JsonProperty("originalImage")]
        public string OriginalImage { get; set; }

        [JsonProperty("faces")]
        public List<string> Faces { get; set; }

    }
}

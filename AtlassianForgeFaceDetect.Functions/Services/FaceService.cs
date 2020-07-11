using AtlassianForgeFaceDetect.Functions.Abstractions;
using AtlassianForgeFaceDetect.Functions.Common;
using AtlassianForgeFaceDetect.Functions.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AtlassianForgeFaceDetect.Functions.Services
{
    public class FaceService : IFaceService
    {
        private readonly IFaceClient _faceClient;
        public FaceService(IFaceClient faceClient)
        {
            _faceClient = faceClient;
        }

        public List<DetectedFace> GetFaces(string url)
        {
            IList<DetectedFace> detectedFaces;

            // Detect faces with all attributes from image url.
            detectedFaces = _faceClient.Face.DetectWithUrlAsync(url,
                    returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Accessories, FaceAttributeType.Age,
                FaceAttributeType.Blur, FaceAttributeType.Emotion, FaceAttributeType.Exposure, FaceAttributeType.FacialHair,
                FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.Hair, FaceAttributeType.HeadPose,
                FaceAttributeType.Makeup, FaceAttributeType.Noise, FaceAttributeType.Occlusion, FaceAttributeType.Smile }).Result;

            return detectedFaces.ToList();
        }
    }
}

using AtlassianForgeFaceDetect.Functions.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Collections.Generic;

namespace AtlassianForgeFaceDetect.Functions.Abstractions
{
    public interface IFaceService
    {
        List<DetectedFace> GetFaces(string url);
    }
}
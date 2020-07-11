using System;

using System.Collections.Generic;
using System.Text;

namespace AtlassianForgeFaceDetect.Functions.Common
{
    public static class AppSettings
    {
        public static string ComputerVisionEndpoint = nameof(ComputerVisionEndpoint);
        public static string ComputerVisionKey = nameof(ComputerVisionKey);
        public static string FaceKey = nameof(FaceKey);
        public static string FaceEndpoint = nameof(FaceEndpoint);
        public static string AzureWebJobsStorage = nameof(AzureWebJobsStorage);
    }
}

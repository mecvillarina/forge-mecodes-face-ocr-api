namespace AtlassianForgeFaceDetect.Functions.Services.Abstractions
{
    public interface IOcrService
    {
        string GetText(byte[] byteData);
    }
}
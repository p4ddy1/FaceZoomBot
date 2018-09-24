using System.Runtime.InteropServices.ComTypes;

namespace FaceZoomBot.DataStorage
{
    public interface IStorage
    {
        string SaveImage(System.IO.Stream imageStream, string fileName);
        string GenerateFacePath(string sourceImagePath);
        string GetFaceBasePath(string sourceImagePath);
        string GenerateFileName();
        string GetBasePath();
        IStream LoadImage(string fileName);
    }
}
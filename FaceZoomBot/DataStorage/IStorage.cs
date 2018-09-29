using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace FaceZoomBot.DataStorage
{
    public interface IStorage
    {
        void SaveImage(Image<Rgb24> image, string imageIdentifier);
        void SaveFace(Image<Rgb24> image, string imageIdentifier);
        Image<Rgb24> LoadImage(string imageIdentifier);
        Image<Rgb24> LoadFace(string imageIdentifier, string faceIdentifier);
        void DeleteImage(string imageIdentifier);
        void DeleteFace(string imageIdentifier, string faceIdentifier);
        void DeleteAllFacesForImage(string imageIdentifier);
        List<string> GetAllFaceIdentifiersForImage(string imageIdentifier);
        string GetRandomIdentifier();
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using FaceZoomBot.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace FaceZoomBot.DataStorage
{
    public class FileSystemStorage : IStorage
    {
        private Config Config { get; }
        private string BasePath { get; }
        private string FacesBasePath { get; }
        private IImageEncoder Encoder { get; }
        private IImageDecoder Decoder { get; }
        
        public FileSystemStorage()
        {
            var factory = new Factory();
            Config = factory.LoadConfig();
            
            BasePath = Path.Combine(Config.General.DataDirectoryPath, "images");

            CreateDirectory(BasePath);
            
            FacesBasePath = Path.Combine(Config.General.DataDirectoryPath, "faces");

            CreateDirectory(FacesBasePath);
            
            Encoder = new JpegEncoder();
            Decoder = new JpegDecoder();;
        }
        
        public void SaveImage(Image<Rgb24> image, string imageIdentifier)
        {
            var imagePath = Path.Combine(BasePath, imageIdentifier);
            using (var saveStream = File.Open(imagePath, FileMode.Create))
            {
                image.Save(saveStream, Encoder);
            }
        }

        public void SaveFace(Image<Rgb24> image, string imageIdentifier)
        {
            var facesPathForImage = Path.Combine(FacesBasePath, imageIdentifier);
            CreateDirectory(facesPathForImage);
            var facePath = Path.Combine(facesPathForImage, GetRandomIdentifier());
            using (var saveStream = File.Open(facePath, FileMode.Create))
            {
                image.Save(saveStream, Encoder);
            }
        }

        public Image<Rgb24> LoadImage(string imageIdentifier)
        {
            var imagePath = Path.Combine(BasePath, imageIdentifier);
            return Image.Load<Rgb24>(imagePath,Decoder);
        }
        
        public Image<Rgb24> LoadFace(string imageIdentifier, string faceIdentifier)
        {
            var facesPathForImage = Path.Combine(FacesBasePath, imageIdentifier);
            var facePath = Path.Combine(facesPathForImage, faceIdentifier);
            return Image.Load<Rgb24>(facePath,Decoder);
        }

        public void DeleteImage(string imageIdentifier)
        {
            var imagePath = Path.Combine(BasePath, imageIdentifier);
            File.Delete(imagePath);
        }

        public void DeleteFace(string imageIdentifier, string faceIdentifier)
        {
            var facesPathForImage = Path.Combine(FacesBasePath, imageIdentifier);
            var facePath = Path.Combine(facesPathForImage, faceIdentifier);
            File.Delete(facePath);
        }

        public void DeleteAllFacesForImage(string imageIdentifier)
        {
            var facesPathForImage = Path.Combine(FacesBasePath, imageIdentifier);
            Directory.Delete(facesPathForImage, true);
        }

        public List<string> GetAllFaceIdentifiersForImage(string imageIdentifier)
        {
            var identifierList = new List<string>();
            var facesPathForImage = Path.Combine(FacesBasePath, imageIdentifier);
            foreach (var facePath in Directory.GetFiles(facesPathForImage))
            {
                identifierList.Add(facePath);
            }
            return identifierList;
        }

        public void CreateDirectory(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        public string GetRandomIdentifier()
        {
            using (var algorithm = SHA384.Create())
            {
                var randomBytes = new byte[1024];
                var random = new Random();
                random.NextBytes(randomBytes);
                var hash = algorithm.ComputeHash(randomBytes);
                return BitConverter.ToString(hash)
                    .Replace("-", "")
                    .ToLower();
            }
        }
    }
}
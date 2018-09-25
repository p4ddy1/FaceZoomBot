using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using FaceZoomBot.Configuration;

namespace FaceZoomBot.DataStorage
{
    public class FileSystemStorage
    {
        private string BasePath { get; set; }
        private string FacesPath { get; }
        private Config Config { get; }

        public FileSystemStorage()
        {
            var factory = new Factory();
            Config = factory.LoadConfig();

            BasePath = Path.Combine(Config.General.DataDirectoryPath, "images");

            if (Directory.Exists(BasePath) == false)
            {
                Directory.CreateDirectory(BasePath);
            }

            FacesPath = Path.Combine(Config.General.DataDirectoryPath, "faces");

            if (Directory.Exists(FacesPath) == false)
            {
                Directory.CreateDirectory(FacesPath);
            }
        }

        public string SaveImage(Stream imageStream, string fileName)
        {
            var imagePath = Path.Combine(BasePath, fileName);
            using (var saveStream = File.Open(imagePath, FileMode.Create))
            {
                imageStream.Seek(0, SeekOrigin.Begin);
                imageStream.CopyTo(saveStream);
            }

            return fileName;
        }

        public void DeleteImage(string fileName)
        {
            var imagePath = Path.Combine(BasePath, fileName);
            File.Delete(imagePath);
        }

        public string GetFaceBasePath(string sourceImagePath)
        {
            var facesRoot = Path.Combine(FacesPath, sourceImagePath);
            return facesRoot;
        }

        public string GenerateFacePath(string sourceImagePath)
        {
            var facesRoot = Path.Combine(FacesPath, sourceImagePath);
            if (Directory.Exists(facesRoot) == false)
            {
                Directory.CreateDirectory(facesRoot);
            }

            return Path.Combine(facesRoot, GenerateFileName());
        }

        public string GetBasePath()
        {
            return BasePath;
        }

        public string GenerateFileName()
        {
            var algorithm = SHA384.Create();
            var randomBytes = new byte[1024];
            var random = new Random();
            random.NextBytes(randomBytes);
            var hash = algorithm.ComputeHash(randomBytes);
            return BitConverter.ToString(hash)
                .Replace("-", "")
                .ToLower();
        }

        public IStream LoadImage(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
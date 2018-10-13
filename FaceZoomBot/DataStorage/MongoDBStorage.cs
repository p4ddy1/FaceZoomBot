using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FaceZoomBot.Models.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace FaceZoomBot.DataStorage
{
    public class MongoDBStorage : IStorage
    {
        private MongoClient MongoClient { get; }
        private IMongoDatabase Database { get; }
        private IMongoCollection<TelegramImage> ImageCollection { get; }
        private IMongoCollection<Face> FaceCollection { get; }
        
        public MongoDBStorage()
        {
            RegisterClassMaps();
            MongoClient = new MongoClient("mongodb://devel:devel@127.0.0.1");
            Database = MongoClient.GetDatabase("faceZoomBot");
            ImageCollection = Database.GetCollection<TelegramImage>("image");
            FaceCollection = Database.GetCollection<Face>("face");
        }

        private void RegisterClassMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Face)))
            {
                BsonClassMap.RegisterClassMap<Face>();
            }
            
            if (!BsonClassMap.IsClassMapRegistered(typeof(TelegramImage)))
            {
                BsonClassMap.RegisterClassMap<TelegramImage>();
            }
        }

        public void SaveImage(Image<Rgb24> image, string imageIdentifier)
        {
            var imageModel = new TelegramImage(
                new ObjectId(imageIdentifier),
                image
            );
            ImageCollection.InsertOne(imageModel);
        }

        public void SaveFace(Image<Rgb24> image, string imageIdentifier)
        {
            var faceModel = new Face(
                imageIdentifier,
                image
            );
            FaceCollection.InsertOne(faceModel);
        }

        public Image<Rgb24> LoadImage(string imageIdentifier)
        {
            var imageModel = ImageCollection.Find(
                new BsonDocument("_id", new ObjectId(imageIdentifier))
            ).FirstOrDefault();
            return imageModel.Image;
        }

        public Image<Rgb24> LoadFace(string imageIdentifier, string faceIdentifier)
        {
            var faceModel = FaceCollection.Find(
                new BsonDocument("_id", new ObjectId(faceIdentifier))
            ).FirstOrDefault();
            return faceModel.Image;
        }

        public void DeleteImage(string imageIdentifier)
        {
            //TODO: Implement deletion
            return;
        }

        public void DeleteFace(string imageIdentifier, string faceIdentifier)
        {
            //TODO: Implement deletion
            return;
        }

        public void DeleteAllFacesForImage(string imageIdentifier)
        {
            //TODO: Implement deletion
            return;
        }

        public List<string> GetAllFaceIdentifiersForImage(string imageIdentifier)
        {
            var identifierList = new List<string>();
            var fieldsBuilder = Builders<Face>.Projection;
            var fields = fieldsBuilder.Exclude(f => f.Image);
            var filter = new BsonDocument("ImageId", imageIdentifier);
            using (var cursor = FaceCollection.Find(filter).Project(fields).ToCursor())
            {
                while (cursor.MoveNext())
                {
                    foreach (var face in cursor.Current)
                    {
                        identifierList.Add(face["_id"].ToString());
                    }
                }
            }
            return identifierList;
        }

        public string GetRandomIdentifier()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
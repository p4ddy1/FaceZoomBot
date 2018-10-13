using System;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace FaceZoomBot.Models.MongoDB
{
    public class Face
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }
        
        [BsonElement]
        public string ImageId { get; set; }
        
        [BsonElement]
        [BsonSerializer(typeof(ImageSerializer))]
        public Image<Rgb24> Image { get; set; }

        public Face(string imageId, Image<Rgb24> image)
        {
            ImageId = imageId;
            Image = image;
        }
    }
}
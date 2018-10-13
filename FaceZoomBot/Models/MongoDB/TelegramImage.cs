using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace FaceZoomBot.Models.MongoDB
{
    public class TelegramImage
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }
        
        [BsonElement]
        [BsonSerializer(typeof(ImageSerializer))]
        public Image<Rgb24> Image { get; set; }

        public TelegramImage(ObjectId id, Image<Rgb24> image)
        {
            Id = id;
            Image = image;
        }
    }
}
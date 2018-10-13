using System.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace FaceZoomBot.Models.MongoDB
{
    public class ImageSerializer : SerializerBase<Image<Rgb24>>
    {
        private IImageDecoder Decoder { get; }
        private IImageEncoder Encoder { get; }

        public ImageSerializer()
        {
            Decoder = new PngDecoder();
            Encoder = new PngEncoder();
        }
        
        public override Image<Rgb24> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Image.Load<Rgb24>(context.Reader.ReadBytes(), Decoder);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Image<Rgb24> value)
        {
            using (var memoryStream = new MemoryStream())
            {
                value.Save(memoryStream, Encoder);
                context.Writer.WriteBytes(memoryStream.ToArray());
            }
        }
    }
}
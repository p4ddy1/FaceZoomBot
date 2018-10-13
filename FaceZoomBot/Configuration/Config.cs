using System.IO;
using Newtonsoft.Json;

namespace FaceZoomBot.Configuration
{
    public class Config
    {
        public General General { get; set; }
        public Telegram Telegram { get; set; }
        public RabbitMQ RabbitMQ { get; set; }
        public MongoDB MongoDB { get; set; }

        public Config(Telegram telegram, RabbitMQ rabbitMQ, General general)
        {
            General = general;
            Telegram = telegram;
            RabbitMQ = rabbitMQ;
        }

        public void Save(string path)
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            File.WriteAllText(path, JsonConvert.SerializeObject(this, jsonSerializerSettings));
        }

        public static Config Load(string path)
        {
            using (var file = File.OpenText(path))
            {
                var jsonSerializer = new JsonSerializer();
                return (Config) jsonSerializer.Deserialize(file, typeof(Config));
            }
        }
    }
}
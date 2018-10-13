using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FaceZoomBot.Configuration
{
    public class General
    {
        public string ShapePredictorPath { get; set; }
        public string DataDirectoryPath { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StorageType StorageType { get; set; }

        public General(string shapePredictorPath, string dataDirectoryPath, StorageType storageType)
        {
            ShapePredictorPath = shapePredictorPath;
            DataDirectoryPath = dataDirectoryPath;
            StorageType = storageType;
        }
    }
}
namespace FaceZoomBot.Configuration
{
    public class General
    {
        public string ShapePredictorPath { get; set; }
        public string DataDirectoryPath { get; set; }

        public General(string shapePredictorPath, string dataDirectoryPath)
        {
            ShapePredictorPath = shapePredictorPath;
            DataDirectoryPath = dataDirectoryPath;
        }
    }
}
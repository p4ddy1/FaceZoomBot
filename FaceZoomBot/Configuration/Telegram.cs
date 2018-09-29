namespace FaceZoomBot.Configuration
{
    public class Telegram
    {
        public string ApiKey { get; set; }

        public Telegram(string apiKey)
        {
            ApiKey = apiKey;
        }
    }
}
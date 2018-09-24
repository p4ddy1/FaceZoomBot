using FaceZoomBot.Configuration;
using Telegram.Bot;

namespace FaceZoomBot.Telegram
{
    public class TelegramClient
    {
        public TelegramBotClient BotClient { get; }
        private Config Config { get; }
        
        public TelegramClient()
        {
            var factory = new Factory();
            Config = factory.LoadConfig();
            
            BotClient = new TelegramBotClient(Config.Telegram.ApiKey);
        }

        public void SendMessage(long chatId, string text)
        {
            BotClient.SendTextMessageAsync(chatId, text);
        }
    }
}
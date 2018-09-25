namespace FaceZoomBot.Models
{
    public class TelegramChat
    {
        public long ChatId { get; set; }
        public bool IsPrivate { get; set; }
        public string Message { get; set; }

        public TelegramChat(long chatId, bool isPrivate, string message = "")
        {
            ChatId = chatId;
            IsPrivate = isPrivate;
            Message = message;
        }
    }
}
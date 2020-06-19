using AnnieMayDiscordBot.Enums;

namespace AnnieMayDiscordBot.Models
{
    public class EmbedMedia
    {
        public string DiscordName { get; set; }
        public EmbedMediaListStatus Status { get; set; }
        public float Score { get; set; }
        public int Progress { get; set; }
    }
}
using AnnieMayDiscordBot.Enums;

namespace AnnieMayDiscordBot.Models
{
    public class EmbedMedia
    {
        public string discordName { get; set; }
        public EmbedMediaListStatus status { get; set; }
        public float score { get; set; }
        public int progress { get; set; }
    }
}

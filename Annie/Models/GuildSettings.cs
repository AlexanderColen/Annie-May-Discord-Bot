namespace AnnieMayDiscordBot.Models
{
    public class GuildSettings
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public string Prefix { get; set; }
        public bool ShowUserScores { get; set; }

        public GuildSettings()
        {
            ShowUserScores = true;
        }
    }
}
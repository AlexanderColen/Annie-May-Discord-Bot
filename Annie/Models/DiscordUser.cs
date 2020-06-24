namespace AnnieMayDiscordBot.Models
{
    public class DiscordUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ulong DiscordId { get; set; }
        public string AnilistName { get; set; }
        public long AnilistId { get; set; }
    }
}
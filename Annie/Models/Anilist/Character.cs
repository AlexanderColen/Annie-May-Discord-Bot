namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Character
    {
        public int id { get; set; }
        public CharacterName name { get; set; }
        public CharacterImage image { get; set; }
        public string description { get; set; }
        public string siteUrl { get; set; }
        public MediaConnection media { get; set; }
        public int favourites { get; set; }
    }
}
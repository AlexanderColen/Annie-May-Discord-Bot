namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Studio
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isAnimationStudio { get; set; }
        public MediaConnection media { get; set; }
        public string siteUrl { get; set; }
        public int favourites { get; set; }
    }
}
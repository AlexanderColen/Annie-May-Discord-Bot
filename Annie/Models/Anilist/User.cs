namespace AnnieMayDiscordBot.Models.Anilist
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string about { get; set; }
        public UserAvatar avatar { get; set; }
        public string bannerImage { get; set; }
        public UserStatisticTypes statistics { get; set; }
        public string siteUrl { get; set; }
        public int updatedAt { get; set; }
    }
}

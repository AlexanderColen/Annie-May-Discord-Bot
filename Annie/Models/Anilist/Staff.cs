using AnnieMayDiscordBot.Enums.Anilist;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Staff
    {
        public int id { get; set; }
        public StaffName name { get; set; }
        public StaffLanguage language { get; set; }
        public StaffImage image { get; set; }
        public string description { get; set; }
        public string siteUrl { get; set; }
        public MediaConnection staffMedia { get; set; }
        public CharacterConnection characters { get; set; }
        public int favourites { get; set; }
    }
}
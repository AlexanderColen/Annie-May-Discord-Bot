using AnnieMayDiscordBot.Enums.Anilist;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Staff
    {
        public int Id { get; set; }
        public StaffName Name { get; set; }
        public StaffLanguage Language { get; set; }
        public StaffImage Image { get; set; }
        public string Description { get; set; }
        public string SiteUrl { get; set; }
        public MediaConnection StaffMedia { get; set; }
        public CharacterConnection Characters { get; set; }
        public int Favourites { get; set; }
    }
}
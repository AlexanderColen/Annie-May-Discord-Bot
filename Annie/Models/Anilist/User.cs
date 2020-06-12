namespace AnnieMayDiscordBot.Models.Anilist
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public UserAvatar Avatar { get; set; }
        public string BannerImage { get; set; }
        public UserStatisticTypes Statistics { get; set; }
        public string SiteUrl { get; set; }
        public int UpdatedAt { get; set; }
    }
}
namespace AnnieMayDiscordBot.Models.Anilist
{
    public class PageInfo
    {
        public int Total { get; set; }
        public int PerPage { get; set; }
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
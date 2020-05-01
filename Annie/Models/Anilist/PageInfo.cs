namespace AnnieMayDiscordBot.Models.Anilist
{
    public class PageInfo
    {
        public int total { get; set; }
        public int perPage { get; set; }
        public int currentPage { get; set; }
        public int lastPage { get; set; }
        public bool hasNextPage { get; set; }
    }
}

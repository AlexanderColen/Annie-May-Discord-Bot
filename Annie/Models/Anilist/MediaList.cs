using AnnieMayDiscordBot.Enums.Anilist;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class MediaList
    {
        public int Id { get; set; }
        public int MediaId { get; set; }
        public MediaListStatus? Status { get; set; }
        public float Score { get; set; }
        public int Progress { get; set; }
        public int ProgressVolumes { get; set; }
        public int Repeat { get; set; }
        public int Priority { get; set; }
        public FuzzyDate StartedAt { get; set; }
        public FuzzyDate CompletedAt { get; set; }
        public int UpdatedAt { get; set; }
        public int CreatedAt { get; set; }
        public Media Media { get; set; }
    }
}
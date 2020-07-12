namespace AnnieMayDiscordBot.Models.Anilist
{
    public abstract class AnilistSpecialUserStatistics
    {
        public int Count { get; set; }
        public float MeanScore { get; set; }
        public int MinutesWatched { get; set; }
        public int ChaptersRead { get; set; }
    }
}
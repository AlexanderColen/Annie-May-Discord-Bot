namespace AnnieMayDiscordBot.Models.Anilist
{
    public class UserStatistics
    {
        public int Count { get; set; }
        public float MeanScore { get; set; }
        public float StandardDeviation { get; set; }
        public int MinutesWatched { get; set; }
        public int EpisodesWatched { get; set; }
        public int ChaptersRead { get; set; }
        public int VolumesRead { get; set; }
    }
}
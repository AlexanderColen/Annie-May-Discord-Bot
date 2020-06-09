namespace AnnieMayDiscordBot.Models.Anilist
{
    public class UserStatistics
    {
        public int count { get; set; }
        public float meanScore { get; set; }
        public float standardDeviation { get; set; }
        public int minutesWatched { get; set; }
        public int episodesWatched { get; set; }
        public int chaptersRead { get; set; }
        public int volumesRead { get; set; }
    }
}
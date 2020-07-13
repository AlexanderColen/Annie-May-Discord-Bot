using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class UserStatistics : AnilistSpecialUserStatistics
    {
        public float StandardDeviation { get; set; }
        public int EpisodesWatched { get; set; }
        public int VolumesRead { get; set; }
        public List<UserFormatStatistic> Formats { get; set; }
        public List<UserStatusStatistic> Statuses { get; set; }
        public List<UserReleaseYearStatistic> ReleaseYears { get; set; }
        public List<UserStartYearStatistic> StartYears { get; set; }
        public List<UserGenreStatistic> Genres { get; set; }
    }
}
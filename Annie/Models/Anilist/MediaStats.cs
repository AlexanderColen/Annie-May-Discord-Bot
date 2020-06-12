using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class MediaStats
    {
        public List<ScoreDistribution> ScoreDistribution { get; set; }
        public List<StatusDistribution> StatusDistribution { get; set; }
    }
}
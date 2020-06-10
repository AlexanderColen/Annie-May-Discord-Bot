using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class MediaStats
    {
        public List<ScoreDistribution> scoreDistribution { get; set; }
        public List<StatusDistribution> statusDistribution { get; set; }
    }
}
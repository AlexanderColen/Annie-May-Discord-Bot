using System;
using System.Collections.Generic;
using System.Text;

namespace AnnieMayDiscordBot.Models
{
    public class MediaStats
    {
        public List<ScoreDistribution> scoreDistribution { get; set; }
        public List<StatusDistribution> statusDistribution { get; set; }
    }
}

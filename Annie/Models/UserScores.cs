using AnnieMayDiscordBot.Models.Anilist;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models
{
    public class UserScores
    {
        public int UpperBound { get; set; }
        public int LowerBound { get; set; }
        public float Count { get; set; }
        public bool HasSAO { get; set; }
        public List<string> MediaTitles { get; set; }
    }
}
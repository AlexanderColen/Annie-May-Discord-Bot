using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public abstract class AnilistName
    {
        public string first { get; set; }
        public string last { get; set; }
        public string full { get; set; }
        public string native { get; set; }
        public List<string> alternative { get; set; }
    }
}

using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public abstract class AnilistName
    {
        public string First { get; set; }
        public string Last { get; set; }
        public string Full { get; set; }
        public string Native { get; set; }
        public List<string> Alternative { get; set; }
    }
}
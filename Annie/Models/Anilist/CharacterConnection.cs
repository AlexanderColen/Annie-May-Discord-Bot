using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class CharacterConnection
    {
        public List<CharacterEdge> edges { get; set; }
        public List<Character> nodes { get; set; }
        public PageInfo pageInfo { get; set; }
    }
}

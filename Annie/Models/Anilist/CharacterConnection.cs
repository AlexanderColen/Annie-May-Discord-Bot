using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class CharacterConnection
    {
        public List<CharacterEdge> Edges { get; set; }
        public List<Character> Nodes { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
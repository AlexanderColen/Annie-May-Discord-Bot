using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class StaffConnection
    {
        public List<StaffEdge> Edges { get; set; }
        public List<Staff> Nodes { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
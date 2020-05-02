using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class StaffConnection
    {
        public List<StaffEdge> edges { get; set; }
        public List<Staff> nodes { get; set; }
        public PageInfo pageInfo { get; set; }
    }
}

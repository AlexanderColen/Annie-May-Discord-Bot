using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class MediaConnection
    {
        public List<MediaEdge> Edges { get; set; }
        public List<Media> Nodes { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
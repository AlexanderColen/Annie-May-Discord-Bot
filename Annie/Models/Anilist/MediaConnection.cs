using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class MediaConnection
    {
        public List<MediaEdge> edges { get; set; }
        public List<Media> nodes { get; set; }
        public PageInfo pageInfo { get; set; }
    }
}

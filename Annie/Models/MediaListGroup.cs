using AnnieMayDiscordBot.Enums;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models
{
    public class MediaListGroup
    {
        public List<MediaList> entries { get; set; }
        public string name { get; set; }
        public MediaListStatus status { get; set; }
    }
}

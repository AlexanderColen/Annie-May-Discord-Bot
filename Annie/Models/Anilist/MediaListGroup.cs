using AnnieMayDiscordBot.Enums.Anilist;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class MediaListGroup
    {
        public List<MediaList> entries { get; set; }
        public string name { get; set; }
        public MediaListStatus? status { get; set; }
    }
}
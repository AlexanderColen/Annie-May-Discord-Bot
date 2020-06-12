using AnnieMayDiscordBot.Enums.Anilist;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class MediaListGroup
    {
        public List<MediaList> Entries { get; set; }
        public string Name { get; set; }
        public MediaListStatus? Status { get; set; }
    }
}
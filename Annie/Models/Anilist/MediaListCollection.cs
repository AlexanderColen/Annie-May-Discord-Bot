using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class MediaListCollection
    {
        public List<MediaListGroup> Lists { get; set; }
        public User User { get; set; }
    }
}
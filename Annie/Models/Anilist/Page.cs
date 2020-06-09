using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Page
    {
        public PageInfo pageInfo { get; set; }
        public List<Media> media { get; set; }
    }
}
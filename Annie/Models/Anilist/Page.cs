using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Page
    {
        public PageInfo pageInfo { get; set; }
        public List<User> users { get; set; }
        public List<Media> media { get; set; }
        public List<Character> characters { get; set; }
        public List<Staff> staff { get; set; }
        public List<Studio> studios { get; set; }
    }
}
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Page
    {
        public PageInfo PageInfo { get; set; }
        public List<User> Users { get; set; }
        public List<Media> Media { get; set; }
        public List<Character> Characters { get; set; }
        public List<Staff> Staff { get; set; }
        public List<Studio> Studios { get; set; }
    }
}
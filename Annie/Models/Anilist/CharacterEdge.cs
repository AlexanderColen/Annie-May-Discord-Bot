using AnnieMayDiscordBot.Enums.Anilist;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class CharacterEdge
    {
        public Character Node { get; set; }
        public int Id { get; set; }
        public CharacterRole Role { get; set; }
        public List<Staff> VoiceActors { get; set; }
        public List<Media> Media { get; set; }
        public int FavouriteOrder { get; set; }
    }
}
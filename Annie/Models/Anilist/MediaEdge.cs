using AnnieMayDiscordBot.Enums.Anilist;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class MediaEdge
    {
        public Media node { get; set; }
        public int id { get; set; }
        public MediaRelation relationType { get; set; }
        public bool isMainStudio { get; set; }
        public List<Character> characters { get; set; }
        public CharacterRole characterRole { get; set; }
        public string staffRole { get; set; }
        public List<Staff> voiceActors { get; set; }
        public int favouriteOrder { get; set; }
    }
}
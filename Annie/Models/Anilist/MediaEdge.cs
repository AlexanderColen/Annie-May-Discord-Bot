using AnnieMayDiscordBot.Enums.Anilist;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class MediaEdge
    {
        public Media Node { get; set; }
        public int Id { get; set; }
        public MediaRelation RelationType { get; set; }
        public bool IsMainStudio { get; set; }
        public List<Character> Characters { get; set; }
        public CharacterRole CharacterRole { get; set; }
        public string StaffRole { get; set; }
        public List<Staff> VoiceActors { get; set; }
        public int FavouriteOrder { get; set; }
    }
}
namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Favourites
    {
        public MediaConnection anime { get; set; }
        public MediaConnection manga { get; set; }
        public CharacterConnection characters { get; set; }
        public StaffConnection staff { get; set; }
        public StudioConnection studios { get; set; }
    }
}
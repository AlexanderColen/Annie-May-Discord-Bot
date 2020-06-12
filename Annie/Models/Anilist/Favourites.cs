namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Favourites
    {
        public MediaConnection Anime { get; set; }
        public MediaConnection Manga { get; set; }
        public CharacterConnection Characters { get; set; }
        public StaffConnection Staff { get; set; }
        public StudioConnection Studios { get; set; }
    }
}
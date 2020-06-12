namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Character
    {
        public int Id { get; set; }
        public CharacterName Name { get; set; }
        public CharacterImage Image { get; set; }
        public string Description { get; set; }
        public string SiteUrl { get; set; }
        public MediaConnection Media { get; set; }
        public int Favourites { get; set; }
    }
}
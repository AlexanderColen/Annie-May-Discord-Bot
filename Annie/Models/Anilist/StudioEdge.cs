namespace AnnieMayDiscordBot.Models.Anilist
{
    public class StudioEdge
    {
        public Studio node { get; set; }
        public int id { get; set; }
        public bool isMain { get; set; }
        public int favouriteOrder { get; set; }
    }
}
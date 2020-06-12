namespace AnnieMayDiscordBot.Models.Anilist
{
    public class StudioEdge
    {
        public Studio Node { get; set; }
        public int Id { get; set; }
        public bool IsMain { get; set; }
        public int FavouriteOrder { get; set; }
    }
}
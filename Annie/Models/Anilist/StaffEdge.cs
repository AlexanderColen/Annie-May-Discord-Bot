namespace AnnieMayDiscordBot.Models.Anilist
{
    public class StaffEdge
    {
        public Staff node { get; set; }
        public int id { get; set; }
        public string role { get; set; }
        public int favouriteOrder { get; set; }
    }
}
namespace AnnieMayDiscordBot.Models.Anilist
{
    public class StaffEdge
    {
        public Staff Node { get; set; }
        public int Id { get; set; }
        public string Role { get; set; }
        public int FavouriteOrder { get; set; }
    }
}
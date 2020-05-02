using AnnieMayDiscordBot.Models.Anilist;
using MongoDB.Bson.Serialization.Attributes;

namespace AnnieMayDiscordBot.Models
{
    public class DiscordUser
    {
        [BsonId]
        public object _id { get; set; }
        [BsonElement]
        public string name { get; set; }
        [BsonElement]
        public ulong discordId { get; set; }
        [BsonElement]
        public string anilistName { get; set; }
        [BsonElement]
        public long anilistId { get; set; }
        //[BsonElement]
        //public MediaListCollection cachedList { get; set; }
    }
}

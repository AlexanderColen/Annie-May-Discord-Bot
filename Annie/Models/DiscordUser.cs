using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnnieMayDiscordBot.Models
{
    public class DiscordUser
    {
        // Constructor that generates a new UUID.
        public DiscordUser() {
            Id = ObjectId.GenerateNewId();
        }

        [BsonId]
        public ObjectId Id { get; set; }

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
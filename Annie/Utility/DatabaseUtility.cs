using AnnieMayDiscordBot.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Utility
{
    public class DatabaseUtility
    {
        private static MongoClient DBClient = new MongoClient(Properties.Resources.MONGO_DB_URI);

        /// <summary>
        /// Get a list of all the Users from the database.
        /// </summary>
        /// <returns>A list of all the DiscordUsers.</returns>
        public async Task<List<DiscordUser>> GetUsersAsync()
        {
            IMongoDatabase db = DBClient.GetDatabase("AnnieMayBot");
            var usersCollection = db.GetCollection<DiscordUser>("users");
            var usersResult = await usersCollection.FindAsync(new BsonDocument());
            return usersResult.ToList();
        }

        /// <summary>
        /// Get a specific User from the database.
        /// </summary>
        /// <param name="discordID">The ID of a Discord user.</param>
        /// <returns>The first DiscordUser found, otherwise null.</returns>
        public async Task<DiscordUser> GetSpecificUserAsync(ulong discordID)
        {
            IMongoDatabase db = DBClient.GetDatabase("AnnieMayBot");
            var usersCollection = db.GetCollection<DiscordUser>("users");
            var filter = Builders<DiscordUser>.Filter.Eq("discordId", discordID);
            var usersResult = await usersCollection.FindAsync(filter);
            var users = usersResult.ToList();
            if (users.Count > 0)
            {
                return users[0];
            }

            return null;
        }

        /// <summary>
        /// Insert or update a User in the database.
        /// </summary>
        /// <param name="discordUser">The DiscordUser to insert or update.</param>
        /// <returns>True if the action was successful, false otherwise.</returns>
        public async Task<bool> UpsertUserAsync(DiscordUser discordUser)
        {
            IMongoDatabase db = DBClient.GetDatabase("AnnieMayBot");
            var usersCollection = db.GetCollection<DiscordUser>("users");
            var filter = Builders<DiscordUser>.Filter.Eq("discordId", discordUser.discordId);
            await usersCollection.ReplaceOneAsync(filter, discordUser, new ReplaceOptions { IsUpsert = true });

            return true;
        }
    }
}
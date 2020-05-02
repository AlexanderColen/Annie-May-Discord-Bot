using AnnieMayDiscordBot.Properties;
using AnnieMayDiscordBot.Services;
using AnnieMayDiscordBot.Utility;
using Discord.Commands;
using MongoDB.Driver;

namespace AnnieMayDiscordBot.Modules
{
    public abstract class AbstractModule : ModuleBase<SocketCommandContext>
    {
        protected AniListFetcher _aniListFetcher = new AniListFetcher();
        protected EmbedUtility _embedUtility = new EmbedUtility();
        protected LevenshteinUtility _levenshteinUtility = new LevenshteinUtility();
        protected MongoClient _dbClient = new MongoClient(Resources.MONGO_DB_URI);
        protected MongoDBUtility _mongoDbUtility = new MongoDBUtility();
    }
}

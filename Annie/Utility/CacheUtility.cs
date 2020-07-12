using AnnieMayDiscordBot.Models;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Utility
{
    // Singleton class.
    public class CacheUtility
    {
        private static readonly CacheUtility _cacheUtilityInstance = new CacheUtility();
        private CacheUtility() {
            CachedGuildSettings = new Dictionary<ulong, GuildSettings>();
        }

        public static CacheUtility GetInstance() => _cacheUtilityInstance;

        public Dictionary<ulong, GuildSettings> CachedGuildSettings { get; }
    }
}
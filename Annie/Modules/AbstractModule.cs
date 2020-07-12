using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Services;
using AnnieMayDiscordBot.Utility;
using Discord.Commands;

namespace AnnieMayDiscordBot.Modules
{
    public abstract class AbstractModule : ModuleBase<CustomCommandContext>
    {
        protected AniListFetcher _aniListFetcher = new AniListFetcher();
        protected EmbedUtility _embedUtility = new EmbedUtility();
        protected LevenshteinUtility _levenshteinUtility = new LevenshteinUtility();
    }
}
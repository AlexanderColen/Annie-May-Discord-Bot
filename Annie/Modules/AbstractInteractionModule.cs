using AnnieMayDiscordBot.Services;
using AnnieMayDiscordBot.Utility;
using Discord.Interactions;

namespace AnnieMayDiscordBot.Modules
{
    public abstract class AbstractInteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        protected AniListFetcher _aniListFetcher = new AniListFetcher();
        protected EmbedUtility _embedUtility = new EmbedUtility();
        protected LevenshteinUtility _levenshteinUtility = new LevenshteinUtility();

        public InteractionService Commands { get; set; }
    }
}
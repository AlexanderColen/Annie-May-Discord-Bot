using AnnieMayDiscordBot.Services;
using AnnieMayDiscordBot.Utility;
using Discord.Commands;

namespace AnnieMayDiscordBot.Modules
{
    public abstract class AbstractModule : ModuleBase<SocketCommandContext>
    {
        protected AniListFetcher _aniListFetcher = new AniListFetcher();
        protected EmbedUtility _embedUtility = new EmbedUtility();
    }
}

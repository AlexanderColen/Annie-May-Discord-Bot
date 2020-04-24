using AnnieMayDiscordBot.Services;
using AnnieMayDiscordBot.Utility;
using Discord.Commands;

namespace AnnieMayDiscordBot.Modules
{
    public abstract class AbstractModule : ModuleBase<SocketCommandContext>
    {
        public AniListFetcher _aniListFetcher = new AniListFetcher();
        public EmbedUtility _embedUtility = new EmbedUtility();
    }
}

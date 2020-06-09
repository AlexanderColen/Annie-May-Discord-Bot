using AnnieMayDiscordBot.ResponseModels;
using Discord.Commands;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class StudioModule : AbstractModule
    {
        [Command("studio")]
        [Summary("Find a studio from AniList GraphQL based on string criteria.")]
        public async Task FindStudioAsync([Remainder] string searchCriteria)
        {
            StudioResponse studioResponse = await _aniListFetcher.FindStudioAsync(searchCriteria);
            await ReplyAsync("", false, _embedUtility.BuildAnilistStudioEmbed(studioResponse.studio));
        }

        [Command("studio")]
        [Summary("Find a studio from AniList GraphQL based on anilist studio id.")]
        public async Task FindStudioAsync([Remainder] int studioId)
        {
            StudioResponse studioResponse = await _aniListFetcher.FindStudioAsync(studioId);
            await ReplyAsync("", false, _embedUtility.BuildAnilistStudioEmbed(studioResponse.studio));
        }
    }
}
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord.Commands;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("studio")]
    public class StudioModule : AbstractModule
    {
        /// <summary>
        /// Look for a Studio entry from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        [Command]
        [Summary("Find a studio from AniList GraphQL based on string criteria.")]
        public async Task FindStudioAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchStudiosAsync(searchCriteria);
            Studio studio = _levenshteinUtility.GetSingleBestStudioResult(searchCriteria, pageResponse.Page.Studios);
            await ReplyAsync("", false, _embedUtility.BuildAnilistStudioEmbed(studio));
        }

        /// <summary>
        /// Look for a Studio entry from Anilist GraphQL database using Studio ID.
        /// </summary>
        /// <param name="studioId">The ID of the Studio entry.</param>
        /// <returns></returns>
        [Command]
        [Summary("Find a studio from AniList GraphQL based on anilist studio id.")]
        public async Task FindStudioAsync([Remainder] int studioId)
        {
            StudioResponse studioResponse = await _aniListFetcher.FindStudioAsync(studioId);
            await ReplyAsync("", false, _embedUtility.BuildAnilistStudioEmbed(studioResponse.Studio));
        }
    }
}
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord.Interactions;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class StudioModule : AbstractInteractionModule
    {
        /// <summary>
        /// Look for a Studio entry from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="args">The criteria to search for.</param>
        [SlashCommand("studio", "Find a studio from AniList GraphQL based on string criteria or ID.")]
        public async Task FindStudioAsync(
            [Summary(name: "search-criteria-or-id", description: "The search criteria to look for or the AniList ID of the studio.")] string args)
        {
            // Defer to give some time to calculate.
            await DeferAsync();

            if (int.TryParse(args, out int studioId))
            {
                StudioResponse studioResponse = await _aniListFetcher.FindStudioAsync(studioId);
                await ModifyOriginalResponseAsync(x => x.Embed = _embedUtility.BuildAnilistStudioEmbed(studioResponse.Studio));
            } else
            {
                PageResponse pageResponse = await _aniListFetcher.SearchStudiosAsync(args);
                Studio studio = _levenshteinUtility.GetSingleBestStudioResult(args, pageResponse.Page.Studios);
                await ModifyOriginalResponseAsync(x => x.Embed = _embedUtility.BuildAnilistStudioEmbed(studio));
            }            
        }
    }
}
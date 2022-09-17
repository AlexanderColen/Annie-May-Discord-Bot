using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord.Interactions;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("studio", "Find information about a studio from AniList.")]
    public class StudioModule : AbstractInteractionModule
    {
        /// <summary>
        /// Look for a Studio entry from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="args">The criteria to search for.</param>
        [SlashCommand("", "Find a studio from AniList GraphQL based on string criteria or ID.")]
        public async Task FindStudioAsync(
            [Summary(name: "search-criteria-or-id", description: "The search criteria to look for or the AniList ID of the studio.")] string args)
        {
            if (int.TryParse(args, out int studioId))
            {
                StudioResponse studioResponse = await _aniListFetcher.FindStudioAsync(studioId);
                await RespondAsync(isTTS: false, embed: _embedUtility.BuildAnilistStudioEmbed(studioResponse.Studio));
            } else
            {
                PageResponse pageResponse = await _aniListFetcher.SearchStudiosAsync(args);
                Studio studio = _levenshteinUtility.GetSingleBestStudioResult(args, pageResponse.Page.Studios);
                await RespondAsync(isTTS: false, embed: _embedUtility.BuildAnilistStudioEmbed(studio));
            }            
        }
    }
}
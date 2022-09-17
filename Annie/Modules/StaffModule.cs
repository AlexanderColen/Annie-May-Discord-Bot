using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord.Interactions;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("staff", "Find information about a staff member from AniList.")]
    public class StaffModule : AbstractInteractionModule
    {
        /// <summary>
        /// Look for a Staff entry from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="args">The criteria to search for.</param>
        [SlashCommand("", "Find a staff member from AniList GraphQL based on string criteria.")]
        public async Task FindStaffAsync(
            [Summary(name: "search-criteria-or-id", description: "The search criteria to look for or the AniList ID of the staff member.")] string args)
        {
            if (int.TryParse(args, out int staffId))
            {
                StaffResponse staffResponse = await _aniListFetcher.FindStaffAsync(staffId);
                await RespondAsync(isTTS: false, embed: _embedUtility.BuildAnilistStaffEmbed(staffResponse.Staff)); 
            } else {
                PageResponse pageResponse = await _aniListFetcher.SearchStaffAsync(args);
                Staff staff = _levenshteinUtility.GetSingleBestStaffResult(args, pageResponse.Page.Staff);
                await RespondAsync(isTTS: false, embed: _embedUtility.BuildAnilistStaffEmbed(staff));
            }
        }
    }
}
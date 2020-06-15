using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord.Commands;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("staff")]
    public class StaffModule : AbstractModule
    {
        /// <summary>
        /// Look for a Staff entry from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        [Command]
        [Summary("Find a staff from AniList GraphQL based on string criteria.")]
        public async Task FindStaffAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchStaffAsync(searchCriteria);
            Staff staff = _levenshteinUtility.GetSingleBestStaffResult(searchCriteria, pageResponse.Page.Staff);
            await ReplyAsync("", false, _embedUtility.BuildAnilistStaffEmbed(staff));
        }

        /// <summary>
        /// Look for a Staff entry from Anilist GraphQL database using Staff ID.
        /// </summary>
        /// <param name="staffId">The ID of the Staff entry.</param>
        [Command]
        [Summary("Find a staff from AniList GraphQL based on anilist staff id.")]
        public async Task FindStaffAsync([Remainder] int staffId)
        {
            StaffResponse staffResponse = await _aniListFetcher.FindStaffAsync(staffId);
            await ReplyAsync("", false, _embedUtility.BuildAnilistStaffEmbed(staffResponse.Staff));
        }
    }
}
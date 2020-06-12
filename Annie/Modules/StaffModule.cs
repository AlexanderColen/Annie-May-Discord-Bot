using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels;
using Discord.Commands;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("staff")]
    public class StaffModule : AbstractModule
    {
        [Command]
        [Summary("Find a staff from AniList GraphQL based on string criteria.")]
        public async Task FindStaffAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchStaffAsync(searchCriteria);
            Staff staff = _levenshteinUtility.GetSingleBestStaffResult(searchCriteria, pageResponse.page.staff);
            await ReplyAsync("", false, _embedUtility.BuildAnilistStaffEmbed(staff));
        }

        [Command]
        [Summary("Find a staff from AniList GraphQL based on anilist staff id.")]
        public async Task FindStaffAsync([Remainder] int staffId)
        {
            StaffResponse staffResponse = await _aniListFetcher.FindStaffAsync(staffId);
            await ReplyAsync("", false, _embedUtility.BuildAnilistStaffEmbed(staffResponse.staff));
        }
    }
}
using AnnieMayDiscordBot.ResponseModels;
using Discord.Commands;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class StaffModule : AbstractModule
    {
        [Command("staff")]
        [Summary("Find a staff from AniList GraphQL based on string criteria.")]
        public async Task FindStaffAsync([Remainder] string searchCriteria)
        {
            StaffResponse staffResponse = await _aniListFetcher.FindStaffAsync(searchCriteria);
            await ReplyAsync("", false, _embedUtility.BuildAnilistStaffEmbed(staffResponse.staff));
        }

        [Command("staff")]
        [Summary("Find a staff from AniList GraphQL based on anilist staff id.")]
        public async Task FindStaffAsync([Remainder] int staffId)
        {
            StaffResponse staffResponse = await _aniListFetcher.FindStaffAsync(staffId);
            await ReplyAsync("", false, _embedUtility.BuildAnilistStaffEmbed(staffResponse.staff));
        }
    }
}

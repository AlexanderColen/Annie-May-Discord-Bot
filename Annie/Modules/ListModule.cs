using AnnieMayDiscordBot.ResponseModels;
using Discord.Commands;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class ListModule : AbstractModule
    {
        [Command("user")]
        [Summary("Find a user's statistics.")]
        public async Task GetAnimeListAsync([Remainder] string username)
        {
            UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(username);

            await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.user));
        }
    }
}

using System.Threading.Tasks;
using AnnieMayDiscordBot.ResponseModels;
using Discord.Commands;

namespace AnnieMayDiscordBot.Modules
{
    public class ListModule : AbstractModule
    {
        [Command("user")]
        [Summary("Find a user's statistics using their username.")]
        public async Task GetAnimeListAsync([Remainder] string username)
        {
            UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(username);

            await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.user));
        }

        [Command("user")]
        [Summary("Find a user's statistics using their id.")]
        public async Task GetAnimeListAsync([Remainder] int userId)
        {
            UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(userId);

            await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.user));
        }
    }
}

using AnnieMayDiscordBot.ResponseModels.Anilist;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Commands;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class ListModule : AbstractModule
    {
        /// <summary>
        /// Display the User's Anilist information.
        /// </summary>
        [Command("user")]
        [Summary("Find a user's statistics without any parameters.")]
        [Alias("list", "userlist", "anilist")]
        public async Task GetUserAniListAsync()
        {
            var user = await DatabaseUtility.GetInstance().GetSpecificUserAsync(Context.User.Id);
            if (user == null)
            {
                await Context.Channel.SendMessageAsync("You're not in my records... Please make sure to setup first using `setup anilist <username/id>`.");
                return;
            }

            try
            {
                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(user.AnilistId);
                await ReplyAsync("", false, _embedUtility.BuildAnilistUserEmbed(userResponse.User));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }

        /// <summary>
        /// Display a specific User's Anilist information using an Anilist username.
        /// </summary>
        /// <param name="username">An Anilist username.</param>
        [Command("user")]
        [Summary("Find a user's statistics using their username.")]
        [Alias("list", "userlist", "anilist")]
        public async Task GetUserAniListAsync([Remainder] string username)
        {
            try
            {
                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(username);
                await ReplyAsync("", false, _embedUtility.BuildAnilistUserEmbed(userResponse.User));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }

        /// <summary>
        /// Display a specific User's Anilist information using an Anilist User ID.
        /// </summary>
        /// <param name="id">An Discord/Anilist User ID.</param>
        [Command("user")]
        [Summary("Find a user's statistics using their id.")]
        [Alias("list", "userlist", "anilist")]
        public async Task GetUserAniListAsync([Remainder] long id)
        {
            var userId = await ModuleUtility.GetInstance().GetAnilistIDAsync(id);

            if (userId.HasValue)
            {
                await ReplyAsync($"Could not find {id} in the database.");
                return;
            }

            try
            {
                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(userId.Value);
                await ReplyAsync("", false, _embedUtility.BuildAnilistUserEmbed(userResponse.User));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }

        /// <summary>
        /// Display a specific User's Anilist information using a tagged Discord User.
        /// </summary>
        /// <param name="user">A tagged Discord User.</param>
        [Command("user")]
        [Summary("Find a user's statistics using their username.")]
        [Alias("list", "userlist", "anilist")]
        public async Task GetUserAniListAsync([Remainder] IUser user)
        {
            try
            {
                var foundUser = await DatabaseUtility.GetInstance().GetSpecificUserAsync(user.Id);
                if (foundUser == null)
                {
                    await ReplyAsync("This filthy weeb isn't in the database.");
                    return;
                }

                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(foundUser.AnilistId);
                await ReplyAsync("", false, _embedUtility.BuildAnilistUserEmbed(userResponse.User));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }
    }
}
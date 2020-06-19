using AnnieMayDiscordBot.ResponseModels.Anilist;
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
            var user = await _databaseUtility.GetSpecificUserAsync(Context.User.Id);
            if (user == null)
            {
                await Context.Channel.SendMessageAsync("You're not in my records... Please make sure to setup first using `setup anilist <username/id>`.");
                return;
            }

            try
            {
                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(user.anilistId);
                await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.User));
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
                await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.User));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }

        /// <summary>
        /// Display a specific User's Anilist information using an Anilist User ID.
        /// </summary>
        /// <param name="userId">An Anilist User ID.</param>
        [Command("user")]
        [Summary("Find a user's statistics using their id.")]
        [Alias("list", "userlist", "anilist")]
        public async Task GetUserAniListAsync([Remainder] long userId)
        {
            // Check if the given int parameter is a Discord User ID (17-18 characters long).
            if (userId.ToString().Length >= 17)
            {
                var user = await _databaseUtility.GetSpecificUserAsync((ulong)userId);
                if (user == null)
                {
                    await ReplyAsync("This filthy weeb isn't in the database.");
                    return;
                }
                // Overwrite the userId with the found Anilist ID.
                userId = user.anilistId;
            }

            try
            {
                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(userId);
                await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.User));
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
                var foundUser = await _databaseUtility.GetSpecificUserAsync(user.Id);
                if (foundUser == null)
                {
                    await ReplyAsync("This filthy weeb isn't in the database.");
                    return;
                }

                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(foundUser.anilistId);
                await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.User));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }
    }
}
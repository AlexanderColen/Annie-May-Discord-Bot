using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Commands;
using System.Collections.Generic;
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
        public async Task GetUserAniListAsync(string username)
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
        public async Task GetUserAniListAsync(long id)
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
        public async Task GetUserAniListAsync(IUser user)
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

        /// <summary>
        /// Find all the registered Anilist Users in the guild.
        /// </summary>
        [Command("users")]
        [Summary("Find all the registered Anilist Users in the guild.")]
        [Alias("guildusers")]
        public async Task GetAllUsers()
        {
            var guildUsers = await Context.Guild.GetUsersAsync();
            var databaseUsers = await DatabaseUtility.GetInstance().GetUsersAsync();
            var registeredUsers = new List<DiscordUser>();

            foreach (var guildUser in guildUsers)
            {
                foreach (var dbUser in databaseUsers)
                {
                    if (guildUser.Id == dbUser.DiscordId)
                    {
                        registeredUsers.Add(dbUser);
                        break;
                    }
                }
            }

            await ReplyAsync(null, false, _embedUtility.BuildUsersEmbed(registeredUsers, Context.Guild));
        }
    }
}
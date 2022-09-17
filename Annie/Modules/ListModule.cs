using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Interactions;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class ListModule : AbstractInteractionModule
    {
        public class UserModule : AbstractInteractionModule
        {
            /// <summary>
            /// Display the User's Anilist information.
            /// </summary>
            [SlashCommand("user", "Find a user's AniList statistics.")]
            public async Task GetUserAniListAsync(
                [Summary(name: "anilist-name-or-id", description: "The AniList user's name or ID to look for.")] string args = null)
            {
                try
                {
                    if (args == null)
                    {
                        var user = await DatabaseUtility.GetInstance().GetSpecificUserAsync(Context.User.Id);
                        if (user == null)
                        {
                            await RespondAsync(text: "You're not in my records... Please make sure to setup first using `setup anilist <username/id>`.", isTTS: false, ephemeral: true);
                            return;
                        }

                        UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(user.AnilistId);
                        await RespondAsync(isTTS: false, embed: _embedUtility.BuildAnilistUserEmbed(userResponse.User));
                    } else if (int.TryParse(args, out int id)) {
                        var userId = await ModuleUtility.GetInstance().GetAnilistIDAsync(id);

                        if (userId.HasValue)
                        {
                            await RespondAsync(text: $"Could not find {id} in the database.");
                            return;
                        }

                        UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(userId.Value);
                        await RespondAsync(isTTS: false, embed: _embedUtility.BuildAnilistUserEmbed(userResponse.User));
                    } else
                    {
                        UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(args);
                        await RespondAsync(isTTS: false, embed: _embedUtility.BuildAnilistUserEmbed(userResponse.User));
                    }
                }
                catch (HttpRequestException)
                {
                    await RespondAsync(text: "Sorry, I could not find this Anilist user.");
                }
            }

            /// <summary>
            /// Display a specific User's Anilist information using a tagged Discord User.
            /// </summary>
            /// <param name="user">A tagged Discord User.</param>
            [UserCommand("user-info")]
            public async Task GetUserAniListAsync(IUser user)
            {
                try
                {
                    var foundUser = await DatabaseUtility.GetInstance().GetSpecificUserAsync(user.Id);
                    if (foundUser == null)
                    {
                        await RespondAsync(text:  "This filthy weeb isn't in the database.");
                        return;
                    }

                    UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(foundUser.AnilistId);
                    await RespondAsync(isTTS: false, embed: _embedUtility.BuildAnilistUserEmbed(userResponse.User));
                }
                catch (HttpRequestException)
                {
                    await RespondAsync(text: "Sorry, I could not find this Anilist user.");
                }
            }
        }

        /// <summary>
        /// Find all the registered Anilist Users in the guild.
        /// </summary>
        [SlashCommand("users", "Find all the registered Anilist Users in the guild.")]
        public async Task GetAllUsers()
        {
            await Context.Guild.DownloadUsersAsync();
            var guildUsers = Context.Guild.Users;
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

            await RespondAsync(isTTS: false, embed: _embedUtility.BuildUsersEmbed(registeredUsers, Context.Guild));
        }
    }
}
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
                            await FollowupAsync(text: "You're not in my records... Please make sure to setup first using `setup anilist <username/id>`.", isTTS: false, ephemeral: true);
                            return;
                        }

                        UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(user.AnilistId);
                        await FollowupAsync(isTTS: false, embed: _embedUtility.BuildAnilistUserEmbed(userResponse.User));
                    } else if (int.TryParse(args, out int id)) {
                        var userId = await ModuleUtility.GetInstance().GetAnilistIDAsync(id);

                        if (userId.HasValue)
                        {
                            await FollowupAsync(text: $"Could not find {id} in the database.", isTTS: false);
                            return;
                        }

                        UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(userId.Value);
                        await FollowupAsync(isTTS: false, embed: _embedUtility.BuildAnilistUserEmbed(userResponse.User));
                    } else
                    {
                        UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(args);
                        await FollowupAsync(isTTS: false, embed: _embedUtility.BuildAnilistUserEmbed(userResponse.User));
                    }
                }
                catch (HttpRequestException)
                {
                    await FollowupAsync(text: "Sorry, I could not find this Anilist user.", isTTS: false);
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
                        await FollowupAsync(text: "This filthy weeb isn't in the database.", isTTS: false);
                        return;
                    }

                    UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(foundUser.AnilistId);
                    await FollowupAsync(isTTS: false, embed: _embedUtility.BuildAnilistUserEmbed(userResponse.User));
                }
                catch (HttpRequestException)
                {
                    await FollowupAsync(text: "Sorry, I could not find this Anilist user.", isTTS: false);
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

            await FollowupAsync(embed: _embedUtility.BuildUsersEmbed(registeredUsers, Context.Guild));
        }
    }
}
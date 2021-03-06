﻿using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Commands;
using Discord.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("affinity")]
    [Alias("affinities", "similarity", "similarities")]
    public class AffinityModule : AbstractModule
    {
        /// <summary>
        /// Calculate the affinity between the user and every other registered user in the guild.
        /// </summary>
        [Command]
        [Summary("Calculate the affinity between the user and every other registered user in the guild.")]
        public async Task GetGuildAffinityAsync()
        {
            var user = await DatabaseUtility.GetInstance().GetSpecificUserAsync(Context.User.Id);

            if (user == null)
            {
                await ReplyAsync("You need to tell me your Anilist before I can calculate your affinity!\n" +
                    "You can do this using the `setup anilist <ID/USERNAME>` command.", false);
                return;
            }

            await Context.Guild.DownloadUsersAsync();
            var guildUsers = await Context.Guild.GetUsersAsync();
            var dicts = new List<Dictionary<string, object>>();
            for (int i = 0; i < guildUsers.Count; i++)
            {
                // Ignore bots and own user.
                if (guildUsers.ToArray()[i].Id == Context.User.Id || guildUsers.ToArray()[i].IsBot)
                {
                    continue;
                }

                var foundUser = await DatabaseUtility.GetInstance().GetSpecificUserAsync(guildUsers.ToArray()[i].Id);

                if (foundUser != null && foundUser.AnilistId != 0)
                {
                    var dict = await HandleAffinityBetweenUsersAsync((user.AnilistId, foundUser.AnilistId),
                                                                     (null, null),
                                                                     guildUsers.ToArray()[i].Username);
                    if (dict != null)
                    {
                        dicts.Add(dict);
                    }
                }
            }

            // Don't bother with embed if there are no dictionaries.
            if (dicts.Count == 0)
            {
                await ReplyAsync("Could not compute affinity because of the lack of other Anilist users.");
                return;
            }

            await ReplyAsync("", false, _embedUtility.BuildAffinityListEmbed(dicts));
        }

        /// <summary>
        /// Calculate the affinity between the user and every other registered user in the guild.
        /// </summary>
        /// <param name="anilistUsername">An Anilist User.</param>
        [Command]
        [Summary("Calculate the affinity between the specified user and every other registered user in the guild.")]
        public async Task GetGuildAffinityAsync(string anilistUsername)
        {
            await Context.Guild.DownloadUsersAsync();
            var guildUsers = await Context.Guild.GetUsersAsync();
            var dicts = new List<Dictionary<string, object>>();
            for (int i = 0; i < guildUsers.Count; i++)
            {
                // Ignore bots.
                if (guildUsers.ToArray()[i].IsBot)
                {
                    continue;
                }

                var foundUserB = await DatabaseUtility.GetInstance().GetSpecificUserAsync(guildUsers.ToArray()[i].Id);

                if (foundUserB != null && foundUserB.AnilistId != 0 && !anilistUsername.ToLower().Equals(foundUserB.AnilistName.ToLower()))
                {
                    var dict = await HandleAffinityBetweenUsersAsync((0, 0),
                                                                     (anilistUsername, foundUserB.AnilistName),
                                                                     guildUsers.ToArray()[i].Username);
                    if (dict != null)
                    {
                        dicts.Add(dict);
                    }
                }
            }

            // Don't bother with embed if there are no dictionaries.
            if (dicts.Count == 0)
            {
                await ReplyAsync("Could not compute affinity because of the lack of other Anilist users.");
                return;
            }

            await ReplyAsync("", false, _embedUtility.BuildAffinityListEmbed(dicts));
        }

        /// <summary>
        /// Calculate the affinity between the user and every other registered user in the guild.
        /// </summary>
        /// <param name="id">The given ID of the Discord/Anilist User.</param>
        [Command]
        [Summary("Calculate the affinity between the specified user and every other registered user in the guild.")]
        public async Task GetGuildAffinityAsync(long id)
        {
            var userId = await ModuleUtility.GetInstance().GetAnilistIDAsync(id);

            if (!userId.HasValue)
            {
                await ReplyAsync($"Could not find {id} in the database.");
                return;
            }
            
            await Context.Guild.DownloadUsersAsync();
            var guildUsers = await Context.Guild.GetUsersAsync();
            var dicts = new List<Dictionary<string, object>>();
            for (int i = 0; i < guildUsers.Count; i++)
            {
                // Ignore bots and own user.
                if (guildUsers.ToArray()[i].Id == (ulong)userId
                    || guildUsers.ToArray()[i].IsBot)
                {
                    continue;
                }

                var foundUserB = await DatabaseUtility.GetInstance().GetSpecificUserAsync(guildUsers.ToArray()[i].Id);

                if (foundUserB != null && foundUserB.AnilistId != 0 && userId != foundUserB.AnilistId)
                {
                    var dict = await HandleAffinityBetweenUsersAsync((userId.Value, foundUserB.AnilistId),
                                                                     (null, null),
                                                                     guildUsers.ToArray()[i].Username);
                    if (dict != null)
                    {
                        dicts.Add(dict);
                    }
                }
            }

            // Don't bother with embed if there are no dictionaries.
            if (dicts.Count == 0)
            {
                await ReplyAsync("Could not compute affinity because of the lack of other Anilist users.");
                return;
            }

            await ReplyAsync("", false, _embedUtility.BuildAffinityListEmbed(dicts));
        }

        /// <summary>
        /// Calculate the affinity between the user and every other registered user in the guild.
        /// </summary>
        [Command]
        [Summary("Calculate the affinity between the specified user and every other registered user in the guild.")]
        public async Task GetGuildAffinityAsync(IUser user)
        {
            var foundUserA = await DatabaseUtility.GetInstance().GetSpecificUserAsync(user.Id);
            if (foundUserA == null)
            {
                await ReplyAsync($"Could not find {user.Username} in the database.");
                return;
            }
            
            await Context.Guild.DownloadUsersAsync();
            var guildUsers = await Context.Guild.GetUsersAsync();
            var dicts = new List<Dictionary<string, object>>();
            for (int i = 0; i < guildUsers.Count; i++)
            {
                // Ignore bots and own user.
                if (guildUsers.ToArray()[i].Id == user.Id || guildUsers.ToArray()[i].IsBot)
                {
                    continue;
                }

                var foundUserB = await DatabaseUtility.GetInstance().GetSpecificUserAsync(guildUsers.ToArray()[i].Id);

                if (foundUserB != null && foundUserB.AnilistId != 0)
                {
                    var dict = await HandleAffinityBetweenUsersAsync((foundUserA.AnilistId, foundUserB.AnilistId),
                                                                     (null, null),
                                                                     guildUsers.ToArray()[i].Username);
                    if (dict != null)
                    {
                        dicts.Add(dict);
                    }
                }
            }

            // Don't bother with embed if there are no dictionaries.
            if (dicts.Count == 0)
            {
                await ReplyAsync("Could not compute affinity because of the lack of other Anilist users.");
                return;
            }

            await ReplyAsync("", false, _embedUtility.BuildAffinityListEmbed(dicts));
        }

        /// <summary>
        /// Calculate the affinity between two Anilist users.
        /// </summary>
        /// <param name="anilistUserA">The username of the first Anilist user.</param>
        /// <param name="anilistUserB">The username of the second Anilist user.</param>
        [Command]
        [Summary("Calculate the affinity between two Anilist users.")]
        public async Task GetAffinityBetweenTwoUsersAsync(string anilistUserA, string anilistUserB)
        {
            var dict = await HandleAffinityBetweenUsersAsync((0, 0), (anilistUserA, anilistUserB));
            if (dict != null)
            {
                await ReplyAsync(null, false, _embedUtility.BuildAffinityEmbed(dict));
            }
        }

        /// <summary>
        /// Calculate the affinity between two users.
        /// </summary>
        /// <param name="idA">The ID of the first user.</param>
        /// <param name="idB">The ID of the second user.</param>
        [Command]
        [Summary("Calculate the affinity between two Discord/Anilist users.")]
        public async Task GetAffinityBetweenTwoUsersAsync(long idA, long idB)
        {
            var userIdA = await ModuleUtility.GetInstance().GetAnilistIDAsync(idA);

            if (!userIdA.HasValue)
            {
                await ReplyAsync($"Could not find {idA} in the database.");
                return;
            }

            var userIdB = await ModuleUtility.GetInstance().GetAnilistIDAsync(idB);

            if (!userIdB.HasValue)
            {
                await ReplyAsync($"Could not find {idB} in the database.");
                return;
            }

            var dict = await HandleAffinityBetweenUsersAsync((userIdA.Value, userIdB.Value), (null, null));
            if (dict != null)
            {
                await ReplyAsync(null, false, _embedUtility.BuildAffinityEmbed(dict));
            }
        }

        /// <summary>
        /// Calculate the affinity between two users.
        /// </summary>
        /// <param name="userA">The first Discord user.</param>
        /// <param name="userB">The second Discord user.</param>
        [Command]
        [Summary("Calculate the affinity between two Anilist users.")]
        public async Task GetAffinityBetweenTwoUsersAsync(IUser userA, IUser userB)
        {
            var foundUserA = await DatabaseUtility.GetInstance().GetSpecificUserAsync(userA.Id);
            if (foundUserA == null)
            {
                await ReplyAsync($"Could not find {userA.Username} in the database.");
                return;
            }
            
            var foundUserB = await DatabaseUtility.GetInstance().GetSpecificUserAsync(userB.Id);
            if (foundUserB == null)
            {
                await ReplyAsync($"Could not find {userB.Username} in the database.");
                return;
            }

            var dict = await HandleAffinityBetweenUsersAsync((foundUserA.AnilistId, foundUserB.AnilistId), (null, null));
            if (dict != null)
            {
                await ReplyAsync(null, false, _embedUtility.BuildAffinityEmbed(dict));
            }
        }

        /// <summary>
        /// Get the shared media between two Anilist users.
        /// </summary>
        /// <param name="userListsA">The MediaListCollection of the first Anilist user.</param>
        /// <param name="userListsB">The MediaListCollection of the second Anilist user.</param>
        /// <returns>A dictionary containing both user's names and the list of shared media.</returns>
        private Dictionary<string, object> GetSharedMediaAsync(MediaListCollection userListsA, MediaListCollection userListsB)
        {
            var fullMediaListA = new List<MediaList>();
            foreach (var list in userListsA.Lists)
            {
                fullMediaListA.AddRange(list.Entries);
            }

            var fullMediaListB = new List<MediaList>();
            foreach (var list in userListsB.Lists)
            {
                fullMediaListB.AddRange(list.Entries);
            }

            var dict = new Dictionary<string, object>()
            {
                { "userA", userListsA.User },
                { "userB", userListsB.User },
                { "shared", AffinityUtility.GetInstance().GetSharedMedia(fullMediaListA, fullMediaListB) }
            };

            return dict;
        }
        
        /// <summary>
        /// Handle the affinity calculation between two users.
        /// </summary>
        /// <param name="userIds">A named tuple of Anilist User IDs.</param>
        /// <param name="usernames">A named tuple of Anilist usernames.</param>
        /// <param name="discordUsername">The Discord username of the second user if applicable.</param>
        private async Task<Dictionary<string, object>> HandleAffinityBetweenUsersAsync((long idA, long idB) userIds, (string usernameA, string usernameB) usernames, string discordUsername = null)
        {
            MediaListCollectionResponse userListsA = null;
            MediaListCollectionResponse userListsB = null;

            // Try to use User IDs.
            if (userIds.idA != 0 && userIds.idB != 0)
            {
                userListsA = await _aniListFetcher.FindUserLists(userIds.idA);
                userListsB = await _aniListFetcher.FindUserLists(userIds.idB);
            }
            // Otherwise use usernames.
            else if (usernames.usernameA != null && usernames.usernameB != null)
            {
                userListsA = await _aniListFetcher.FindUserLists(usernames.usernameA);
                userListsB = await _aniListFetcher.FindUserLists(usernames.usernameB);
            }

            var animeDict = GetSharedMediaAsync(userListsA.AnimeList, userListsB.AnimeList);
            var mangaDict = GetSharedMediaAsync(userListsA.MangaList, userListsB.MangaList);
            
            if (animeDict.TryGetValue("shared", out object sharedAnime) && 
                mangaDict.TryGetValue("shared", out object sharedManga))
            {
                List<(int, float, float)> sharedMedia = ((List<(int, float, float)>)sharedAnime)
                    .Concat((List<(int, float, float)>)sharedManga)
                    .ToList();

                return new Dictionary<string, object>
                {
                    ["userA"] = animeDict["userA"],
                    ["userB"] = animeDict["userB"],
                    ["discordUsername"] = discordUsername,
                    ["shared"] = sharedMedia,
                    ["affinity"] = AffinityUtility.GetInstance().CalculatePearsonAffinity(sharedMedia)
                };
            }
            
            await ReplyAsync("Failed to compute affinity between these users.", false);
            return null;
        }
    }
}

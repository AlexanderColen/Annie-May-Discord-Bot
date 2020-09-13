using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Commands;
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
                await ReplyAsync("You need to tell me your Anilist before I can calculate your affinity!\nYou can do this using the `setup anilist <ID/USERNAME>` command.", false);
            }

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
                    var animeListsA = await _aniListFetcher.FindUserList(user.AnilistId, MediaType.Anime.ToString());
                    var animeListsB = await _aniListFetcher.FindUserList(foundUser.AnilistId, MediaType.Anime.ToString());
                    var mangaListsA = await _aniListFetcher.FindUserList(user.AnilistId, MediaType.Manga.ToString());
                    var mangaListsB = await _aniListFetcher.FindUserList(foundUser.AnilistId, MediaType.Manga.ToString());

                    var animeDict = GetSharedMediaAsync(animeListsA.MediaListCollection, animeListsB.MediaListCollection);
                    var mangaDict = GetSharedMediaAsync(mangaListsA.MediaListCollection, mangaListsB.MediaListCollection);
                    var dict = CombineDictionaries(animeDict, mangaDict);

                    dicts.Add(dict);
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
        /// <param name="anilistUser">The Anilist </param>
        [Command]
        [Summary("Calculate the affinity between the specified user and every other registered user in the guild.")]
        public async Task GetGuildAffinityAsync(long userId)
        {
            // Check if the first long parameter is a Discord User ID (17-18 digits).
            if (userId.ToString().Length >= 17)
            {
                var user = await DatabaseUtility.GetInstance().GetSpecificUserAsync((ulong)userId);
                if (user == null)
                {
                    await ReplyAsync($"Could not find {userId} in the database.");
                    return;
                }
                // Overwrite the userId with the found Anilist ID.
                userId = user.AnilistId;
            }

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
                    var animeListsA = await _aniListFetcher.FindUserList(userId, MediaType.Anime.ToString());
                    var animeListsB = await _aniListFetcher.FindUserList(foundUserB.AnilistId, MediaType.Anime.ToString());
                    var mangaListsA = await _aniListFetcher.FindUserList(userId, MediaType.Manga.ToString());
                    var mangaListsB = await _aniListFetcher.FindUserList(foundUserB.AnilistId, MediaType.Manga.ToString());

                    var animeDict = GetSharedMediaAsync(animeListsA.MediaListCollection, animeListsB.MediaListCollection);
                    var mangaDict = GetSharedMediaAsync(mangaListsA.MediaListCollection, mangaListsB.MediaListCollection);
                    var dict = CombineDictionaries(animeDict, mangaDict);

                    dicts.Add(dict);
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
                    var animeListsA = await _aniListFetcher.FindUserList(foundUserA.AnilistId, MediaType.Anime.ToString());
                    var animeListsB = await _aniListFetcher.FindUserList(foundUserB.AnilistId, MediaType.Anime.ToString());
                    var mangaListsA = await _aniListFetcher.FindUserList(foundUserA.AnilistId, MediaType.Manga.ToString());
                    var mangaListsB = await _aniListFetcher.FindUserList(foundUserB.AnilistId, MediaType.Manga.ToString());

                    var animeDict = GetSharedMediaAsync(animeListsA.MediaListCollection, animeListsB.MediaListCollection);
                    var mangaDict = GetSharedMediaAsync(mangaListsA.MediaListCollection, mangaListsB.MediaListCollection);
                    var dict = CombineDictionaries(animeDict, mangaDict);

                    dicts.Add(dict);
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
            var animeListsA = await _aniListFetcher.FindUserList(anilistUserA, MediaType.Anime.ToString());
            var animeListsB = await _aniListFetcher.FindUserList(anilistUserB, MediaType.Anime.ToString());
            var mangaListsA = await _aniListFetcher.FindUserList(anilistUserA, MediaType.Manga.ToString());
            var mangaListsB = await _aniListFetcher.FindUserList(anilistUserB, MediaType.Manga.ToString());

            var animeDict = GetSharedMediaAsync(animeListsA.MediaListCollection, animeListsB.MediaListCollection);
            var mangaDict = GetSharedMediaAsync(mangaListsA.MediaListCollection, mangaListsB.MediaListCollection);
            var dict = CombineDictionaries(animeDict, mangaDict);

            await ReplyAsync(null, false, _embedUtility.BuildAffinityEmbed(dict));
        }

        /// <summary>
        /// Calculate the affinity between two users.
        /// </summary>
        /// <param name="userIdA">The ID of the first user.</param>
        /// <param name="userIdB">The ID of the second user.</param>
        [Command]
        [Summary("Calculate the affinity between two Anilist users.")]
        public async Task GetAffinityBetweenTwoUsersAsync(long userIdA, long userIdB)
        {
            // Check if the first long parameter is a Discord User ID (17-18 digits).
            if (userIdA.ToString().Length >= 17)
            {
                var user = await DatabaseUtility.GetInstance().GetSpecificUserAsync((ulong)userIdA);
                if (user == null)
                {
                    await ReplyAsync($"Could not find {userIdA} in the database.");
                    return;
                }
                // Overwrite the userId with the found Anilist ID.
                userIdA = user.AnilistId;
            }

            // Check if the second long parameter is a Discord User ID (17-18 digits).
            if (userIdB.ToString().Length >= 17)
            {
                var user = await DatabaseUtility.GetInstance().GetSpecificUserAsync((ulong)userIdB);
                if (user == null)
                {
                    await ReplyAsync($"Could not find {userIdB} in the database.");
                    return;
                }
                // Overwrite the userId with the found Anilist ID.
                userIdB = user.AnilistId;
            }
            
            var animeListsA = await _aniListFetcher.FindUserList(userIdA, MediaType.Anime.ToString());
            var animeListsB = await _aniListFetcher.FindUserList(userIdB, MediaType.Anime.ToString());
            var mangaListsA = await _aniListFetcher.FindUserList(userIdA, MediaType.Manga.ToString());
            var mangaListsB = await _aniListFetcher.FindUserList(userIdB, MediaType.Manga.ToString());

            var animeDict = GetSharedMediaAsync(animeListsA.MediaListCollection, animeListsB.MediaListCollection);
            var mangaDict = GetSharedMediaAsync(mangaListsA.MediaListCollection, mangaListsB.MediaListCollection);
            var dict = CombineDictionaries(animeDict, mangaDict);

            await ReplyAsync(null, false, _embedUtility.BuildAffinityEmbed(dict));
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
            
            var animeListsA = await _aniListFetcher.FindUserList(foundUserA.AnilistId, MediaType.Anime.ToString());
            var animeListsB = await _aniListFetcher.FindUserList(foundUserB.AnilistId, MediaType.Anime.ToString());
            var mangaListsA = await _aniListFetcher.FindUserList(foundUserA.AnilistId, MediaType.Manga.ToString());
            var mangaListsB = await _aniListFetcher.FindUserList(foundUserB.AnilistId, MediaType.Manga.ToString());

            var animeDict = GetSharedMediaAsync(animeListsA.MediaListCollection, animeListsB.MediaListCollection);
            var mangaDict = GetSharedMediaAsync(mangaListsA.MediaListCollection, mangaListsB.MediaListCollection);
            var dict = CombineDictionaries(animeDict, mangaDict);

            await ReplyAsync(null, false, _embedUtility.BuildAffinityEmbed(dict));
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
        /// Combine the shared media lists within two Dictionaries.
        /// </summary>
        /// <param name="animeDict">The Dictionary containing shared Anime.</param>
        /// <param name="mangaDict">The Dictionary containing shared Manga.</param>
        /// <returns>A new Dictionary combining both shared lists along with calculated affinity.</returns>
        private Dictionary<string, object> CombineDictionaries(Dictionary<string, object> animeDict, Dictionary<string, object> mangaDict)
        {
            animeDict.TryGetValue("shared", out object sharedAnime);
            mangaDict.TryGetValue("shared", out object sharedManga);

            List<(int, float, float)> sharedMedia = ((List<(int, float, float)>)sharedAnime)
                .Concat((List<(int, float, float)>)sharedManga)
                .ToList();

            return new Dictionary<string, object>
            {
                ["userA"] = animeDict["userA"],
                ["userB"] = animeDict["userB"],
                ["shared"] = sharedMedia,
                ["affinity"] = AffinityUtility.GetInstance().CalculatePearsonAffinity(sharedMedia)
            };
        }
    }
}

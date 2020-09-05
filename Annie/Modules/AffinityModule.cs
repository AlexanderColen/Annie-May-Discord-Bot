using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.Utility;
using Discord.Commands;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("affinity")]
    [Alias("similarity", "similarities")]
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

                if (foundUser != null && !foundUser.AnilistName.Equals(""))
                {
                    var dict = await GetSharedMediaAsync(user.AnilistName, foundUser.AnilistName, MediaType.Anime);
                    dict.TryGetValue("shared", out object sharedMedia);
                    dict.Add("affinity", AffinityUtility.GetInstance().CalculatePearsonAffinity((List<(int, float, float)>)sharedMedia));
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
            // Default to Anime for media.
            var dict = await GetSharedMediaAsync(anilistUserA, anilistUserB, MediaType.Anime);

            // Prefetch all the values.
            dict.TryGetValue("shared", out object sharedMedia);
            dict.TryGetValue("userA", out object userA);
            dict.TryGetValue("userB", out object userB);

            // Don't bother with affinity if there is no shared media.
            if (((List<(int, float, float)>)sharedMedia).Count == 0)
            {
                await ReplyAsync("Could not compute affinity because of the lack of shared media.");
                return;
            }

            var affinity = AffinityUtility.GetInstance().CalculatePearsonAffinity((List<(int, float, float)>)sharedMedia);

            await ReplyAsync($"**{(affinity * 100).ToString("N2", CultureInfo.InvariantCulture)}%** affinity between **{((User)userA).Name}** and **{userB}**. " +
                $"_({((List<(int, float, float)>)sharedMedia).Count} shared media entries)_", false);
        }

        /// <summary>
        /// Get the shared media between two Anilist users.
        /// </summary>
        /// <param name="anilistUserA">The username of the first Anilist user.</param>
        /// <param name="anilistUserB">The username of the second Anilist user.</param>
        /// <param name="mediaType">The type of media to search for.</param>
        /// <returns>A dictionary containing both user's names and the list of shared media.</returns>
        private async Task<Dictionary<string, object>> GetSharedMediaAsync(string anilistUserA, string anilistUserB, MediaType mediaType)
        {
            var userListsA = await _aniListFetcher.FindUserList(anilistUserA, mediaType.ToString());
            var userListsB = await _aniListFetcher.FindUserList(anilistUserB, mediaType.ToString());

            var fullMediaListA = new List<MediaList>();
            foreach (var list in userListsA.MediaListCollection.Lists)
            {
                fullMediaListA.AddRange(list.Entries);
            }

            var fullMediaListB = new List<MediaList>();
            foreach (var list in userListsB.MediaListCollection.Lists)
            {
                fullMediaListB.AddRange(list.Entries);
            }

            var dict = new Dictionary<string, object>()
            {
                { "userA", userListsA.MediaListCollection.User },
                { "userB", userListsB.MediaListCollection.User.Name },
                { "shared", AffinityUtility.GetInstance().GetSharedMedia(fullMediaListA, fullMediaListB) }
            };

            return dict;
        }
    }
}

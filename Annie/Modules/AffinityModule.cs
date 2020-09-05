using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.Utility;
using Discord.Commands;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("affinity")]
    [Alias("similarity", "similarities")]
    public class AffinityModule : AbstractModule
    {
        /// <summary>
        /// Calculate the affinity between two Anilist users.
        /// </summary>
        /// <param name="anilistUserA">The username of the first Anilist user.</param>
        /// <param name="anilistUserB">The username of the second Anilist user.</param>
        [Command]
        [Summary("Calculate the affinity between two Anilist users.")]
        public async Task GetAffinity(string anilistUserA, string anilistUserB)
        {
            // Default to Anime for media.
            var userListsA = await _aniListFetcher.FindUserList(anilistUserA, MediaType.Anime.ToString());
            var userListsB = await _aniListFetcher.FindUserList(anilistUserB, MediaType.Anime.ToString());

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

            var sharedMedia = AffinityUtility.GetInstance().GetSharedMedia(fullMediaListA, fullMediaListB);

            var affinity = AffinityUtility.GetInstance().CalculatePearsonAffinity(sharedMedia);

            await ReplyAsync($"**{(affinity * 100).ToString("N2", CultureInfo.InvariantCulture)}%** affinity between **{userListsA.MediaListCollection.User.Name}** and **{userListsB.MediaListCollection.User.Name}**. _({sharedMedia.Count} shared media entries)_", false);
        }
    }
}

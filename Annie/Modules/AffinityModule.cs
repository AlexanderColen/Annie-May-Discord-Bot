using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.Utility;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("affinity")]
    [Alias("similarity", "similarities")]
    public class AffinityModule : AbstractModule
    {
        [Command]
        public async Task GetAffinity(string anilistUserA, string anilistUserB)
        {
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

            var affinity = AffinityUtility.GetInstance().GetSharedMedia(fullMediaListA, fullMediaListB);
            await ReplyAsync($"{affinity.Count}", false);
        }
    }
}

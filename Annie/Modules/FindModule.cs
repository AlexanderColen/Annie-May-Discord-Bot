using System.Linq;
using System.Threading.Tasks;
using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.ResponseModels;
using Discord.Commands;

namespace AnnieMayDiscordBot.Modules
{
    public class FindModule : AbstractModule
    {
        [Command("find")]
        [Alias("fetch")]
        [Summary("Find media from AniList GraphQL.")]
        public async Task FindAsync([Remainder] string searchCriteria)
        {
            // Redirect to anime or manga find based on if there is a second parameter "anime" or "manga".
            if (searchCriteria.Split(' ')[0] == "anime")
            {
                await FindAnimeAsync(string.Join(' ', searchCriteria.Split(' ').Skip(1)));
                return;
            }

            if (searchCriteria.Split(' ')[0] == "manga")
            {
                await FindMangaAsync(string.Join(' ', searchCriteria.Split(' ').Skip(1)));
                return;
            }

            MediaResponse mediaResponse = await _aniListFetcher.FindMediaAsync(searchCriteria);

            await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(mediaResponse.media));
        }

        [Command("anime")]
        [Summary("Find anime media from AniList GraphQL.")]
        public async Task FindAnimeAsync([Remainder] string searchCriteria)
        {
            MediaResponse mediaResponse = await _aniListFetcher.FindMediaTypeAsync(searchCriteria, MediaType.ANIME.ToString());

            await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(mediaResponse.media));
        }

        [Command("manga")]
        [Summary("Find manga media from AniList GraphQL.")]
        public async Task FindMangaAsync([Remainder] string searchCriteria)
        {
            MediaResponse mediaResponse = await _aniListFetcher.FindMediaTypeAsync(searchCriteria, MediaType.MANGA.ToString());

            await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(mediaResponse.media));
        }
    }
}

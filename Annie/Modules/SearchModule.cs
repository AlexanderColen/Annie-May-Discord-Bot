using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels;
using Discord.Commands;

namespace AnnieMayDiscordBot.Modules
{
    [Group("search")]
    public class SearchModule : AbstractModule
    {
        [Command]
        [Summary("Search a list of media from AniList GraphQL.")]
        public async Task SearchAsync([Remainder] string searchCriteria)
        {
            string[] arguments = searchCriteria.Split(' ');
            // Execute differently based on second argument being 'anime', 'manga'.
            switch (arguments[0])
            {
                case "anime":
                    await SearchAnimeAsync(string.Join(' ', arguments.Skip(1)));
                    break;
                case "manga":
                    await SearchMangaAsync(string.Join(' ', arguments.Skip(1)));
                    break;
                default:
                    PageResponse pageResponse = await _aniListFetcher.SearchMediaAsync(searchCriteria);
                    await ReplyWithMedia(pageResponse.page.media);
                    break;
            }
        }

        [Command("anime")]
        [Summary("Search a list of anime media from AniList GraphQL.")]
        public async Task SearchAnimeAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.Anime.ToString());
            await ReplyWithMedia(pageResponse.page.media);
        }

        [Command("manga")]
        [Summary("Search a list of manga media from AniList GraphQL.")]
        public async Task SearchMangaAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.Manga.ToString());
            await ReplyWithMedia(pageResponse.page.media);
        }

        private async Task ReplyWithMedia(List<Media> mediaList)
        {
            // Return out of the method and send a message when there were no results.
            if (mediaList.Count == 0)
            {
                await ReplyAsync("No media found with this search.");
                return;
            }

            // Sort the media on title.
            mediaList = new List<Media>(mediaList.OrderBy(m => m.title.english));

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```\n");

            foreach (Media media in mediaList)
            {
                stringBuilder.Append($"{media.type} {media.id}: {media.title.english ?? media.title.romaji}\n");
            }

            stringBuilder.Append("```\n");

            await ReplyAsync(stringBuilder.ToString());
        }
    }
}

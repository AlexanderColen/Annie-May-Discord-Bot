using AnnieMayDiscordBot.Enums;
using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.ResponseModels;
using Discord.Commands;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("search")]
    public class SearchModule : AbstractModule
    {
        [Command]
        [Summary("Search a list of media from AniList GraphQL.")]
        public async Task SearchAsync([Remainder] string searchCriteria)
        {
            // Redirect to anime or manga search based on if there is a second parameter "anime" or "manga".
            if (searchCriteria.Split(' ')[0] == "anime")
            {
                await SearchAnimeAsync(string.Join(' ', searchCriteria.Split(' ').Skip(1)));
                return;
            }
            else if (searchCriteria.Split(' ')[0] == "manga")
            {
                await SearchMangaAsync(string.Join(' ', searchCriteria.Split(' ').Skip(1)));
                return;
            }

            PageResponse pageResponse = await _aniListFetcher.SearchMediaAsync(searchCriteria);
            // Return out of the method and send a message when there were no results.
            if (pageResponse.page.media.Count == 0)
            {
                await ReplyAsync("No media found with this search.");
                return;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```\n");

            foreach (Media media in pageResponse.page.media)
            {
                stringBuilder.Append($"{media.type} {media.id}: {(media.title.english != null ? media.title.english : media.title.romaji)}\n");
            }

            stringBuilder.Append("```\n");
            await ReplyAsync(stringBuilder.ToString());
        }

        [Command("anime")]
        [Summary("Search a list of anime media from AniList GraphQL.")]
        public async Task SearchAnimeAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.ANIME.ToString());
            // Return out of the method and send a message when there were no results.
            if (pageResponse.page.media.Count == 0)
            {
                await ReplyAsync("No anime found with this search.");
                return;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```\n");

            foreach (Media media in pageResponse.page.media)
            {
                stringBuilder.Append($"{media.type} {media.id}: {(media.title.english != null ? media.title.english : media.title.romaji)}\n");
            }

            stringBuilder.Append("```\n");
            await ReplyAsync(stringBuilder.ToString());
        }

        [Command("manga")]
        [Summary("Search a list of manga media from AniList GraphQL.")]
        public async Task SearchMangaAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.MANGA.ToString());
            // Return out of the method and send a message when there were no results.
            if (pageResponse.page.media.Count == 0)
            {
                await ReplyAsync("No manga found with this search.");
                return;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```\n");

            foreach (Media media in pageResponse.page.media)
            {
                stringBuilder.Append($"{media.type} {media.id}: {(media.title.english != null ? media.title.english : media.title.romaji)}\n");
            }

            stringBuilder.Append("```\n");

            await ReplyAsync(stringBuilder.ToString());
        }
    }
}

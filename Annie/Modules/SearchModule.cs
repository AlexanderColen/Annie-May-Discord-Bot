using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels;
using Discord.Commands;
using System.Collections.Generic;
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

                case "character":
                    await SearchCharactersAsync(string.Join(' ', arguments.Skip(1)));
                    break;

                case "characters":
                    await SearchCharactersAsync(string.Join(' ', arguments.Skip(1)));
                    break;

                case "char":
                    await SearchCharactersAsync(string.Join(' ', arguments.Skip(1)));
                    break;

                case "waifu":
                    await SearchCharactersAsync(string.Join(' ', arguments.Skip(1)));
                    break;

                case "staff":
                    await SearchStaffAsync(string.Join(' ', arguments.Skip(1)));
                    break;

                case "studio":
                    await SearchStudiosAsync(string.Join(' ', arguments.Skip(1)));
                    break;

                case "studios":
                    await SearchStudiosAsync(string.Join(' ', arguments.Skip(1)));
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

        [Command("character")]
        [Summary("Search a list of characters from AniList GraphQL.")]
        [Alias("characters", "waifu", "char")]
        public async Task SearchCharactersAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchCharactersAsync(searchCriteria);
            List<Character> characterList = pageResponse.page.characters;
            // Return out of the method and send a message when there were no results.
            if (characterList.Count == 0)
            {
                await ReplyAsync("No characters found with this search.");
                return;
            }

            // Sort the characters on full name.
            characterList = new List<Character>(characterList.OrderBy(c => c.name.full));

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```\n");

            foreach (Character character in characterList)
            {
                stringBuilder.Append($"{character.id}: {character.name.full} ({character.name.native ?? ""})\n");
            }

            stringBuilder.Append("```\n");

            await ReplyAsync(stringBuilder.ToString());
        }

        [Command("staff")]
        [Summary("Search a list of characters from AniList GraphQL.")]
        public async Task SearchStaffAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchStaffAsync(searchCriteria);
            List<Staff> staffList = pageResponse.page.staff;
            // Return out of the method and send a message when there were no results.
            if (staffList.Count == 0)
            {
                await ReplyAsync("No staff found with this search.");
                return;
            }

            // Sort the staff on full name.
            staffList = new List<Staff>(staffList.OrderBy(s => s.name.full));

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```\n");

            foreach (Staff staff in staffList)
            {
                stringBuilder.Append($"{staff.id}: {staff.name.full} ({staff.name.native ?? ""})\n");
            }

            stringBuilder.Append("```\n");

            await ReplyAsync(stringBuilder.ToString());
        }

        [Command("studio")]
        [Summary("Search a list of characters from AniList GraphQL.")]
        [Alias("studios")]
        public async Task SearchStudiosAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchStudiosAsync(searchCriteria);
            List<Studio> studioList = pageResponse.page.studios;
            // Return out of the method and send a message when there were no results.
            if (studioList.Count == 0)
            {
                await ReplyAsync("No studios found with this search.");
                return;
            }

            // Sort the studios on name.
            studioList = new List<Studio>(studioList.OrderBy(c => c.name));

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```\n");

            foreach (Studio studio in studioList)
            {
                stringBuilder.Append($"{studio.id}: {studio.name}\n");
            }

            stringBuilder.Append("```\n");

            await ReplyAsync(stringBuilder.ToString());
        }

        /// <summary>
        /// Format the Media list items and reply with them to the appropriate channel.
        /// </summary>
        /// <param name="mediaList">The list of Media items.</param>
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
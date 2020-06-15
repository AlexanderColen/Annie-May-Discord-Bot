using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
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
        /// <summary>
        /// Search for entries from Anilist GraphQL database using search criteria.
        /// Defaults to a Media entries unless specified otherwise.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
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
                    await ReplyWithMedia(pageResponse.Page.Media);
                    break;
            }
        }

        /// <summary>
        /// Search for anime Media entries from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        [Command("anime")]
        [Summary("Search a list of anime media from AniList GraphQL.")]
        public async Task SearchAnimeAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.Anime.ToString());
            await ReplyWithMedia(pageResponse.Page.Media);
        }

        /// <summary>
        /// Search for manga Media entries from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        [Command("manga")]
        [Summary("Search a list of manga media from AniList GraphQL.")]
        public async Task SearchMangaAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.Manga.ToString());
            await ReplyWithMedia(pageResponse.Page.Media);
        }

        /// <summary>
        /// Search for Character entries from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        [Command("character")]
        [Summary("Search a list of characters from AniList GraphQL.")]
        [Alias("characters", "waifu", "char")]
        public async Task SearchCharactersAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchCharactersAsync(searchCriteria);
            List<Character> characterList = pageResponse.Page.Characters;
            // Return out of the method and send a message when there were no results.
            if (characterList.Count == 0)
            {
                await ReplyAsync("No characters found with this search.");
                return;
            }

            // Sort the characters on full name.
            characterList = new List<Character>(characterList.OrderBy(c => c.Name.Full));

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```\n");

            foreach (Character character in characterList)
            {
                string appendage = $"{character.Id}: {character.Name.Full}";
                if (!string.IsNullOrEmpty(character.Name.Native))
                {
                    appendage += $" ({character.Name.Native})";
                }
                stringBuilder.Append($"{appendage}\n");
            }

            stringBuilder.Append("```\n");

            await ReplyAsync(stringBuilder.ToString());
        }

        /// <summary>
        /// Search for Staff entries from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        [Command("staff")]
        [Summary("Search a list of characters from AniList GraphQL.")]
        public async Task SearchStaffAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchStaffAsync(searchCriteria);
            List<Staff> staffList = pageResponse.Page.Staff;
            // Return out of the method and send a message when there were no results.
            if (staffList.Count == 0)
            {
                await ReplyAsync("No staff found with this search.");
                return;
            }

            // Sort the staff on full name.
            staffList = new List<Staff>(staffList.OrderBy(s => s.Name.Full));

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```\n");

            foreach (Staff staff in staffList)
            {
                string appendage = $"{staff.Id}: {staff.Name.Full}";
                if (!string.IsNullOrEmpty(staff.Name.Native))
                {
                    appendage += $" ({staff.Name.Native})";
                }
                stringBuilder.Append($"{appendage}\n");
            }

            stringBuilder.Append("```\n");

            await ReplyAsync(stringBuilder.ToString());
        }

        /// <summary>
        /// Search for Studio entries from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        [Command("studio")]
        [Summary("Search a list of characters from AniList GraphQL.")]
        [Alias("studios")]
        public async Task SearchStudiosAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchStudiosAsync(searchCriteria);
            List<Studio> studioList = pageResponse.Page.Studios;
            // Return out of the method and send a message when there were no results.
            if (studioList.Count == 0)
            {
                await ReplyAsync("No studios found with this search.");
                return;
            }

            // Sort the studios on name.
            studioList = new List<Studio>(studioList.OrderBy(c => c.Name));

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```\n");

            foreach (Studio studio in studioList)
            {
                stringBuilder.Append($"{studio.Id}: {studio.Name}\n");
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
            mediaList = new List<Media>(mediaList.OrderBy(m => m.Title.English));

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("```\n");

            foreach (Media media in mediaList)
            {
                stringBuilder.Append($"{media.Type} {media.Id}: {media.Title.English ?? media.Title.Romaji}\n");
            }

            stringBuilder.Append("```\n");

            await ReplyAsync(stringBuilder.ToString());
        }
    }
}
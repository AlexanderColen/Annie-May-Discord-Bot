using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord.Interactions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class SearchModule : AbstractInteractionModule
    {
        public enum SearchType
        {
            Anime,
            Manga,
            Characters,
            Staff,
            Studios
        }

        /// <summary>
        /// Search for anime Media entries from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        [SlashCommand("search", "Search a list of media from AniList GraphQL.")]
        public async Task SearchAsync(SearchType type, [Summary(name: "search-criteria", description: "The criteria to search for")] string args)
        {
            switch (type)
            {
                case SearchType.Anime:
                    await SearchAnimeAsync(args);
                    break;
                    
                case SearchType.Manga:
                    await SearchMangaAsync(args);
                    break;
                    
                case SearchType.Characters:
                    await SearchCharactersAsync(args);
                    break;
                    
                case SearchType.Staff:
                    await SearchStaffAsync(args);
                    break;
                    
                case SearchType.Studios:
                    await SearchStudiosAsync(args);
                    break;

                default:
                    await SearchAnimeAsync(args);
                    break;
            }
        }

        /// <summary>
        /// Search for anime Media entries from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        private async Task SearchAnimeAsync(string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.Anime.ToString());
            await ReplyWithMedia(pageResponse.Page.Media);
        }

        /// <summary>
        /// Search for manga Media entries from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        private async Task SearchMangaAsync(string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.Manga.ToString());
            await ReplyWithMedia(pageResponse.Page.Media);
        }

        /// <summary>
        /// Search for Character entries from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        private async Task SearchCharactersAsync(string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchCharactersAsync(searchCriteria);
            List<Character> characterList = pageResponse.Page.Characters;
            // Return out of the method and send a message when there were no results.
            if (characterList.Count == 0)
            {
                await RespondAsync(text: "No characters found with this search.");
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

            await RespondAsync(text: stringBuilder.ToString());
        }

        /// <summary>
        /// Search for Staff entries from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        private async Task SearchStaffAsync(string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchStaffAsync(searchCriteria);
            List<Staff> staffList = pageResponse.Page.Staff;
            // Return out of the method and send a message when there were no results.
            if (staffList.Count == 0)
            {
                await RespondAsync(text: "No staff found with this search.");
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

            await RespondAsync(text: stringBuilder.ToString());
        }

        /// <summary>
        /// Search for Studio entries from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        private async Task SearchStudiosAsync(string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchStudiosAsync(searchCriteria);
            List<Studio> studioList = pageResponse.Page.Studios;
            // Return out of the method and send a message when there were no results.
            if (studioList.Count == 0)
            {
                await RespondAsync(text: "No studios found with this search.");
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

            await RespondAsync(text: stringBuilder.ToString());
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
                await RespondAsync(text: "No media found with this search.");
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

            await RespondAsync(text: stringBuilder.ToString());
        }
    }
}
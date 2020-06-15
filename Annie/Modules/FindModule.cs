using AnnieMayDiscordBot.Enums;
using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord;
using Discord.Commands;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class FindModule : AbstractModule
    {
        [Command("find")]
        [Alias("fetch", "get", "media")]
        [Summary("Find media from AniList GraphQL using string criteria.")]
        public async Task FindAsync([Remainder] string searchCriteria)
        {
            string[] arguments = searchCriteria.Split(' ');
            // Execute differently based on second argument being 'anime', 'manga'.
            switch (arguments[0])
            {
                // Case for then the first argument was 'anime'; For example "find anime dxd".
                case "anime":
                    // Try to parse the first argument as an integer to see if an Anilist ID was provided.
                    try
                    {
                        int animeId = int.Parse(string.Join(' ', arguments.Skip(1)));
                        await FindAnimeAsync(animeId);
                    }
                    // If exception is thrown, it means that the request was using search criteria instead.
                    catch (FormatException)
                    {
                        await FindAnimeAsync(string.Join(' ', arguments.Skip(1)));
                    }
                    break;
                // Case for then the first argument was 'manga'; For example "find manga dxd".
                case "manga":
                    // Try to parse the first argument as an integer to see if an Anilist ID was provided.
                    try
                    {
                        int mangaId = int.Parse(string.Join(' ', arguments.Skip(1)));
                        await FindMangaAsync(mangaId);
                    }
                    // If exception is thrown, it means that the request was using search criteria instead.
                    catch (FormatException)
                    {
                        await FindMangaAsync(string.Join(' ', arguments.Skip(1)));
                    }
                    break;
                // Default case when the first argument was neither 'anime' nor 'manga'.
                default:
                    PageResponse pageResponse = await _aniListFetcher.SearchMediaAsync(searchCriteria);
                    // Only reply with the media if there was more than 1 media found.
                    if (pageResponse.Page.Media.Count > 0)
                    {
                        Media media = _levenshteinUtility.GetSingleBestMediaResult(searchCriteria, pageResponse.Page.Media);
                        List<EmbedMedia> embedMediaList = await FetchMediaStatsForUsers(media);
                        await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(media, embedMediaList));
                    }
                    // Notify the user of no results otherwise.
                    else
                    {
                        await ReplyAsync("No media found.");
                    }
                    break;
            }
        }

        [Command("anime")]
        [Summary("Find anime media from AniList GraphQL based on string criteria.")]
        public async Task FindAnimeAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.Anime.ToString());
            // Notify the user of no results.
            if (pageResponse.Page.Media.Count == 0)
            {
                await ReplyAsync("No anime found.");
                return;
            }
            Media media = _levenshteinUtility.GetSingleBestMediaResult(searchCriteria, pageResponse.Page.Media);
            List<EmbedMedia> embedMediaList = await FetchMediaStatsForUsers(media);
            await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(media, embedMediaList));
        }

        [Command("anime")]
        [Summary("Find anime media from AniList GraphQL based on anilist anime id.")]
        public async Task FindAnimeAsync([Remainder] int animeId)
        {
            MediaResponse mediaResponse = await _aniListFetcher.FindMediaTypeAsync(animeId, MediaType.Anime.ToString());
            List<EmbedMedia> embedMediaList = await FetchMediaStatsForUsers(mediaResponse.Media);
            await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(mediaResponse.Media, embedMediaList));
        }

        [Command("manga")]
        [Summary("Find manga media from AniList GraphQL.")]
        public async Task FindMangaAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.Manga.ToString());
            // Notify the user of no results.
            if (pageResponse.Page.Media.Count == 0)
            {
                await ReplyAsync("No manga found.");
                return;
            }
            Media media = _levenshteinUtility.GetSingleBestMediaResult(searchCriteria, pageResponse.Page.Media);
            List<EmbedMedia> embedMediaList = await FetchMediaStatsForUsers(media);
            await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(media, embedMediaList));
        }

        [Command("manga")]
        [Summary("Find manga media from AniList GraphQL.")]
        public async Task FindMangaAsync([Remainder] int mangaId)
        {
            MediaResponse mediaResponse = await _aniListFetcher.FindMediaTypeAsync(mangaId, MediaType.Manga.ToString());
            List<EmbedMedia> embedMediaList = await FetchMediaStatsForUsers(mediaResponse.Media);
            await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(mediaResponse.Media, embedMediaList));
        }

        /// <summary>
        /// Fetch the specific Media's scores and status for every user.
        /// </summary>
        /// <param name="media">The Media that the scores and status should be about.</param>
        /// <returns>A list of the user's statusses and scores as EmberMedia objects.</returns>
        private async Task<List<EmbedMedia>> FetchMediaStatsForUsers(Media media)
        {
            // Fetch users from MongoDB collection.
            IMongoDatabase db = _dbClient.GetDatabase("AnnieMayBot");
            var usersCollection = db.GetCollection<DiscordUser>("users");
            var users = await usersCollection.FindAsync(new BsonDocument());

            // Initialize list for future media embeds.
            List<EmbedMedia> embedMediaList = new List<EmbedMedia>();

            // Loop over all the users to potentially add their Media statistics.
            foreach (var user in users.ToList())
            {
                IUser discordUser = await Context.Channel.GetUserAsync(user.discordId);

                // Skip this user if they are not in the server.
                if (discordUser == null)
                {
                    continue;
                }

                try
                {
                    MediaListResponse response = await _aniListFetcher.FindMediaScoresForUser(user.anilistId, media.Id);

                    // Create and return the new EmbedMedia.
                    EmbedMedia embedMedia = new EmbedMedia
                    {
                        discordName = discordUser.Username,
                        progress = response.MediaList.Progress,
                        score = response.MediaList.Score
                    };
                    Enum.TryParse(response.MediaList.Status.ToString(), out EmbedMediaListStatus parsedStatus);
                    embedMedia.status = parsedStatus;
                    embedMediaList.Add(embedMedia);
                } catch (Exception ex)
                {
                    // Return unwatched EmbedMedia if nothing was found.
                    EmbedMedia embedMedia = new EmbedMedia
                    {
                        discordName = discordUser.Username,
                        progress = 0,
                        score = 0,
                        status = EmbedMediaListStatus.Not_On_List
                    };
                    embedMediaList.Add(embedMedia);
                }
            }

            return embedMediaList;
        }
    }
}
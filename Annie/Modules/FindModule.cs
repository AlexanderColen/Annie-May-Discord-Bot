using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnnieMayDiscordBot.Enums;
using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels;
using Discord;
using Discord.Commands;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AnnieMayDiscordBot.Modules
{
    public class FindModule : AbstractModule
    {
        [Command("find")]
        [Alias("fetch", "get", "media")]
        [Summary("Find media from AniList GraphQL using string criteria.")]
        public async Task FindStringAsync([Remainder] string searchCriteria)
        {
            string[] arguments = searchCriteria.Split(' ');
            // Execute differently based on second argument being 'anime', 'manga'.
            switch (arguments[0])
            {
                case "anime":
                    try
                    {
                        int animeId = int.Parse(string.Join(' ', arguments.Skip(1)));
                        await FindAnimeAsync(animeId);
                    }
                    catch (FormatException)
                    {
                        await FindAnimeAsync(string.Join(' ', arguments.Skip(1)));
                    }
                    break;
                case "manga":
                    try
                    {
                        int mangaId = int.Parse(string.Join(' ', arguments.Skip(1)));
                        await FindMangaAsync(mangaId);
                    }
                    catch (FormatException)
                    {
                        await FindMangaAsync(string.Join(' ', arguments.Skip(1)));
                    }
                    break;
                default:
                    PageResponse pageResponse = await _aniListFetcher.SearchMediaAsync(searchCriteria);
                    if (pageResponse.page.media.Count > 0)
                    {
                        Media media = _levenshteinUtility.GetSingleBestResult(searchCriteria, pageResponse.page.media);
                        List<EmbedMedia> embedMediaList = await FetchMediaStatsForUser(media);
                        await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(media, embedMediaList));
                    }
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
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.ANIME.ToString());
            Media media = _levenshteinUtility.GetSingleBestResult(searchCriteria, pageResponse.page.media);
            List<EmbedMedia> embedMediaList = await FetchMediaStatsForUser(media);
            await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(media, embedMediaList));
        }

        [Command("anime")]
        [Summary("Find anime media from AniList GraphQL based on anilist anime id.")]
        public async Task FindAnimeAsync([Remainder] int animeId)
        {
            MediaResponse mediaResponse = await _aniListFetcher.FindMediaTypeAsync(animeId, MediaType.ANIME.ToString());
            List<EmbedMedia> embedMediaList = await FetchMediaStatsForUser(mediaResponse.media);
            await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(mediaResponse.media, embedMediaList));
        }

        [Command("manga")]
        [Summary("Find manga media from AniList GraphQL.")]
        public async Task FindMangaAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(searchCriteria, MediaType.MANGA.ToString());
            Media media = _levenshteinUtility.GetSingleBestResult(searchCriteria, pageResponse.page.media);
            List<EmbedMedia> embedMediaList = await FetchMediaStatsForUser(media);
            await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(media, embedMediaList));
        }
        
        [Command("manga")]
        [Summary("Find manga media from AniList GraphQL.")]
        public async Task FindMangaAsync([Remainder] int mangaId)
        {
            MediaResponse mediaResponse = await _aniListFetcher.FindMediaTypeAsync(mangaId, MediaType.MANGA.ToString());
            List<EmbedMedia> embedMediaList = await FetchMediaStatsForUser(mediaResponse.media);
            await ReplyAsync("", false, _embedUtility.BuildAnilistMediaEmbed(mediaResponse.media, embedMediaList));
        }

        /// <summary>
        /// Fetch the specific Media's scores and status for every user.
        /// </summary>
        /// <param name="media">The Media that the scores and status should be about.</param>
        /// <returns>A list of the user's statusses and scores as EmberMedia objects.</returns>
        private async Task<List<EmbedMedia>> FetchMediaStatsForUser(Media media)
        {
            // Fetch users from MongoDB collection.
            IMongoDatabase db = _dbClient.GetDatabase("AnnieMayBot");
            var usersCollection = db.GetCollection<DiscordUser>("users");
            var users = await usersCollection.FindAsync(new BsonDocument());

            // Initialize list for future media embeds.
            List<EmbedMedia> embedMediaList = new List<EmbedMedia>();

            foreach (var user in users.ToList())
            {
                IUser discordUser = await Context.Channel.GetUserAsync(user.discordId);

                if (discordUser == null)
                {
                    continue;
                }

                int countBefore = embedMediaList.Count;
                var mediaList = await _aniListFetcher.FindUserListAsync(user.anilistId, media.type.ToString());

                foreach (MediaListGroup listGroup in mediaList.mediaListCollection.lists)
                {
                    foreach (MediaList entry in listGroup.entries)
                    {
                        // Add entry with fields if found.
                        if (entry.mediaId.Equals(media.id))
                        {
                            EmbedMedia embedMedia = new EmbedMedia
                            {
                                discordName = discordUser.Username,
                                progress = entry.progress,
                                score = entry.score
                            };
                            Enum.TryParse(entry.status.ToString(), out EmbedMediaListStatus parsedStatus);
                            embedMedia.status = parsedStatus;
                            embedMediaList.Add(embedMedia);
                            break;
                        }
                    }
                }

                // Add unwatched if count is still the same.
                if (countBefore.Equals(embedMediaList.Count))
                {
                    EmbedMedia embedMedia = new EmbedMedia
                    {
                        discordName = discordUser.Username,
                        progress = 0,
                        score = 0,
                        status = EmbedMediaListStatus.NOT_ON_LIST
                    };
                    embedMediaList.Add(embedMedia);
                }
            }

            return embedMediaList;
        }
    }
}

using AnnieMayDiscordBot.Enums;
using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class FindModule : AbstractInteractionModule
    {
        /// <summary>
        /// Find a Media entry from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="args">The criteria to search for.</param>
        [SlashCommand("find", "Find media from AniList GraphQL using string criteria.")]
        public async Task FindAsync(
            [Summary(name: "search-criteria-or-id", description: "The search criteria to look for or the AniList ID of the anime")] string args)
        {
            string[] arguments = args.Split(' ');
            // Execute differently based on second argument being 'anime', 'manga'.
            switch (arguments[0])
            {
                // Case for then the first argument was 'anime'; For example "find anime dxd".
                case "anime":
                    await FindAnimeAsync(string.Join(' ', arguments.Skip(1)));
                    break;

                // Case for then the first argument was 'manga'; For example "find manga dxd".
                case "manga":
                    await FindMangaAsync(string.Join(' ', arguments.Skip(1)));
                    break;

                // Default case when the first argument was neither 'anime' nor 'manga'.
                default:
                    PageResponse pageResponse = await _aniListFetcher.SearchMediaAsync(args);
                    // Only reply with the media if there was more than 1 media found.
                    if (pageResponse.Page.Media.Count > 0)
                    {
                        Media media = _levenshteinUtility.GetSingleBestMediaResult(args, pageResponse.Page.Media);
                        await BuildMediaEmbedAndRespond(media);
                    }
                    // Notify the user of no results otherwise.
                    else
                    {
                        await RespondAsync(text: "No media found.");
                    }
                    break;
            }
        }

        /// <summary>
        /// Find an anime Media entry from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="args">The criteria to search for.</param>
        [SlashCommand("anime", "Find anime media from AniList GraphQL using string criteria or ID.")]
        public async Task FindAnimeAsync(
            [Summary(name: "search-criteria-or-id", description: "The search criteria to look for or the AniList ID of the anime")] string args)
        {
            if (int.TryParse(args, out int animeId))
            {
                MediaResponse mediaResponse = await _aniListFetcher.FindMediaTypeAsync(animeId, MediaType.Anime.ToString());
                await BuildMediaEmbedAndRespond(mediaResponse.Media);
            } else
            {
                PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(args, MediaType.Anime.ToString());
                // Notify the user of no results.
                if (pageResponse.Page.Media.Count == 0)
                {
                    await RespondAsync(text: "No anime found.");
                    return;
                }
                Media media = _levenshteinUtility.GetSingleBestMediaResult(args, pageResponse.Page.Media);
                await BuildMediaEmbedAndRespond(media);
            }
        }

        /// <summary>
        /// Find a manga Media entry from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="args">The criteria to search for.</param>
        [SlashCommand("manga", "Find manga media from AniList GraphQL using string criteria or ID.")]
        public async Task FindMangaAsync(
            [Summary(name: "search-criteria-or-id", description: "The search criteria to look for or the AniList ID of the manga")] string args)
        {
            if (int.TryParse(args, out int mangaId))
            {
                MediaResponse mediaResponse = await _aniListFetcher.FindMediaTypeAsync(mangaId, MediaType.Manga.ToString());
                await BuildMediaEmbedAndRespond(mediaResponse.Media);
            } else
            {
                PageResponse pageResponse = await _aniListFetcher.SearchMediaTypeAsync(args, MediaType.Manga.ToString());
                // Notify the user of no results.
                if (pageResponse.Page.Media.Count == 0)
                {
                    await RespondAsync(text: "No manga found.");
                    return;
                }
                Media media = _levenshteinUtility.GetSingleBestMediaResult(args, pageResponse.Page.Media);
                await BuildMediaEmbedAndRespond(media);
            }
        }

        /// <summary>
        /// Build the embed for a Media item with user scores.
        /// </summary>
        /// <param name="media">The Media that the scores and status should be about.</param>
        /// <returns>A task responding to the command with a styled Media embed.</returns>
        private async Task BuildMediaEmbedAndRespond(Media media)
        {
            List<EmbedMedia> embedMediaList = await FetchMediaStatsForUsers(media);
            
            // Get the settings that should be used for this Guild.
            CacheUtility.GetInstance().CachedGuildSettings.TryGetValue(Context.Guild.Id, out GuildSettings guildSettings);

            await RespondAsync(isTTS: false, embed: _embedUtility.BuildAnilistMediaEmbed(media, embedMediaList, guildSettings == null || guildSettings.ShowUserScores));
        }

        /// <summary>
        /// Fetch the specific Media's scores and status for every user.
        /// </summary>
        /// <param name="media">The Media that the scores and status should be about.</param>
        /// <returns>A list of the user's statusses and scores as EmberMedia objects.</returns>
        private async Task<List<EmbedMedia>> FetchMediaStatsForUsers(Media media)
        {
            // Initialize list for future media embeds.
            List<EmbedMedia> embedMediaList = new List<EmbedMedia>();

            // Loop over all the users to potentially add their Media statistics.
            await Context.Guild.DownloadUsersAsync();
            foreach (var user in await DatabaseUtility.GetInstance().GetUsersAsync())
            {
                IUser discordUser = await Context.Channel.GetUserAsync(user.DiscordId, CacheMode.AllowDownload);

                // Skip this user if they are not in the server.
                if (discordUser == null)
                {
                    continue;
                }

                try
                {
                    MediaListResponse response = await _aniListFetcher.FindMediaScoresForUser(user.AnilistId, media.Id);

                    // Create and return the new EmbedMedia.
                    EmbedMedia embedMedia = new EmbedMedia
                    {
                        DiscordName = discordUser.Username,
                        Progress = response.MediaList.Progress,
                        Score = response.MediaList.Score
                    };
                    Enum.TryParse(response.MediaList.Status.ToString(), out EmbedMediaListStatus parsedStatus);
                    embedMedia.Status = parsedStatus;
                    embedMediaList.Add(embedMedia);
                }
                catch (Exception)
                {
                    // Return unwatched EmbedMedia if nothing was found.
                    EmbedMedia embedMedia = new EmbedMedia
                    {
                        DiscordName = discordUser.Username,
                        Progress = 0,
                        Score = 0,
                        Status = EmbedMediaListStatus.Not_On_List
                    };
                    embedMediaList.Add(embedMedia);
                }
            }

            return embedMediaList;
        }
    }
}
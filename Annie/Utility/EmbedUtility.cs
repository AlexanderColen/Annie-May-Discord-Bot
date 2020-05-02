﻿using AnnieMayDiscordBot.Enums;
using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Models.Anilist;
using Discord;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AnnieMayDiscordBot.Utility
{
    public class EmbedUtility
    {
        public Embed BuildAnilistMediaEmbed(Media media, List<EmbedMedia> embedMediaList)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            string season = media.season != null ? media.season.ToString() : "?";
            string seasonYear = media.seasonYear != null ? media.seasonYear.ToString() : "?";

            // First row.
            embedBuilder.WithTitle(media.title.english ?? media.title.romaji)
                .AddField("**Type**", media.type, true)
                .AddField("**Status**", media.status, true)
                .AddField("**Season**", $"{season} {seasonYear}", true);

            // Second row.
            embedBuilder.AddField("**Anilist Score**", media.meanScore != null ? $"{media.meanScore}/100" : "-", true)
                .AddField("**Popularity**", media.popularity, true)
                .AddField("**Favourited**", $"{media.favourites} times", true);

            // Third row differs for anime and manga.
            if (media.type == MediaType.ANIME)
            {
                embedBuilder.AddField("**Episodes**", media.episodes != null ? $"{media.episodes}" : "?", true)
                    .AddField("**Duration**", media.duration != null ? $"{media.duration} minutes per episode" : "?", true);
            }
            else
            {
                embedBuilder.AddField("**Volumes**", media.volumes != null ? $"{media.volumes}" : "?", true)
                    .AddField("**Chapters**", media.chapters != null ? $"{media.chapters}" : "?", true);
            }

            // Fourth row.
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("`");
            stringBuilder.Append(string.Join("` - `", media.genres));
            stringBuilder.Append("`");
            embedBuilder.AddField("**Genres**", stringBuilder.ToString());

            // Fifth row with User scores.
            if (embedMediaList != null)
            {
                stringBuilder.Clear();

                StringBuilder completedStringBuilder = new StringBuilder();
                StringBuilder plannedStringBuilder = new StringBuilder();
                StringBuilder inProgressStringBuilder = new StringBuilder();
                StringBuilder droppedStringBuilder = new StringBuilder();
                StringBuilder notOnListStringBuilder = new StringBuilder();
                foreach (EmbedMedia embedMedia in embedMediaList?.OrderBy(s => s.progress).ThenBy(s => s.discordName))
                {
                    switch (embedMedia.status)
                    {
                        case EmbedMediaListStatus.COMPLETED:
                            completedStringBuilder.Append($"{embedMedia.discordName} [{embedMedia.score}/100] ~ ");
                            break;
                        case EmbedMediaListStatus.CURRENT:
                            inProgressStringBuilder.Append($"{embedMedia.discordName} [{embedMedia.progress}] ~ ");
                            break;
                        case EmbedMediaListStatus.DROPPED:
                            droppedStringBuilder.Append($"{embedMedia.discordName} [{embedMedia.progress}] ~ ");
                            break;
                        case EmbedMediaListStatus.PAUSED:
                            inProgressStringBuilder.Append($"{embedMedia.discordName} [{embedMedia.progress}] ~ ");
                            break;
                        case EmbedMediaListStatus.PLANNING:
                            plannedStringBuilder.Append($"{embedMedia.discordName} ~ ");
                            break;
                        default:
                            notOnListStringBuilder.Append($"{embedMedia.discordName} ~ ");
                            break;
                    }
                }

                string inProgress = inProgressStringBuilder.ToString().TrimEnd(' ', '~');
                string completed = completedStringBuilder.ToString().TrimEnd(' ', '~');
                string dropped = droppedStringBuilder.ToString().TrimEnd(' ', '~');
                string planned = plannedStringBuilder.ToString().TrimEnd(' ', '~');
                string notOnList = notOnListStringBuilder.ToString().TrimEnd(' ', '~');

                stringBuilder.Append($"**In-Progress**: {inProgress}\n");
                stringBuilder.Append($"**Completed**: {completed}\n");
                stringBuilder.Append($"**Dropped**: {dropped}\n");
                stringBuilder.Append($"**Planned**: {planned}\n");
                stringBuilder.Append($"**Not-On-List**: {notOnList}\n");

                embedBuilder.AddField("**User Scores**", stringBuilder.ToString());
            }

            if (media.description != null)
            {
                // Remove all the HTML elements from the description.
                embedBuilder.WithDescription($"[MyAnimeList Alternative](https://myanimelist.net/anime/{media.idMal})\n\n_{Regex.Replace(media.description, "(<\\/?\\w+>)", " ")}_");
            }

            // Add all extra properties.
            embedBuilder.WithColor(Color.Green)
                //.WithCurrentTimestamp()
                .WithThumbnailUrl(media.coverImage.extraLarge)
                .WithUrl(media.siteUrl);

            return embedBuilder.Build();
        }

        public Embed BuildUserEmbed(User user, bool withAnime = true, bool withManga = true)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            StringBuilder stringBuilder = new StringBuilder();

            // Build custom description for displaying anime
            if (withAnime)
            {
                stringBuilder.Append($"\n[**Anime List**]({user.siteUrl}{"/animelist"})\n");
                stringBuilder.Append($"_Total Entries:_ {user.statistics.anime.count.ToString("N0", CultureInfo.InvariantCulture)}\n");
                stringBuilder.Append($"_Episodes Watched:_ {user.statistics.anime.episodesWatched.ToString("N0", CultureInfo.InvariantCulture)}\n");
                TimeSpan t = TimeSpan.FromMinutes(user.statistics.anime.minutesWatched);
                stringBuilder.Append($"_Time Watched:_ {t.Days:00} Days - {t.Hours:00} Hours - {t.Minutes:00} Minutes\n");
                stringBuilder.Append($"_Mean Score:_ {user.statistics.anime.meanScore.ToString("N2", CultureInfo.InvariantCulture)}\n");
            }

            if (withManga)
            {
                stringBuilder.Append($"\n[**Manga List**]({user.siteUrl}{"/mangalist"})\n");
                if (user.name == "SmellyAlex")
                {
                    stringBuilder.Append("_Total Entries:_ -1\n");
                    stringBuilder.Append("_Volumes Read:_ -1\n");
                    stringBuilder.Append("_Chapters Read:_ -1\n");
                    stringBuilder.Append("_Mean Score:_ -100\n");

                }
                else
                {
                    stringBuilder.Append($"`Total Entries` {user.statistics.manga.count.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"`Volumes Read` {user.statistics.manga.volumesRead.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"`Chapters Read` {user.statistics.manga.chaptersRead.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"`Mean Score` {user.statistics.manga.meanScore.ToString("N2", CultureInfo.InvariantCulture)}\n");
                }
            }
            
            embedBuilder.WithDescription(stringBuilder.ToString());

            // Add all extra properties.
            embedBuilder.WithColor(Color.DarkPurple)
                //.WithCurrentTimestamp()
                .WithImageUrl(user.bannerImage)
                .WithThumbnailUrl(user.avatar.large)
                .WithTitle($"{user.name} AniList Statistics")
                .WithUrl(user.siteUrl);

            return embedBuilder.Build();
        }
    }
}

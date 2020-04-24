using AnnieMayDiscordBot.Enums;
using AnnieMayDiscordBot.Models;
using Discord;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AnnieMayDiscordBot.Utility
{
    public class EmbedUtility
    {
        public Embed BuildAnilistMediaEmbed(Media media)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            // First row.
            embedBuilder.WithTitle(media.title.english != null ? media.title.english : media.title.romaji)
                .AddField("**Type**", media.type, true)
                .AddField("**Anilist Score**", media.meanScore, true)
                .AddField("**Popularity**", media.popularity, true);

            // Second row.
            embedBuilder.AddField("**Status**", media.status, true);

            // Second part of second row differs for anime and manga.
            if (media.type == MediaType.ANIME)
            {
                embedBuilder.AddField("**Episodes**", media.episodes != null ? $"{media.episodes}" : "?", true)
                    .AddField("**Duration**", $"{media.duration} minutes per episode", true);
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
            embedBuilder.AddField("**Genres**", stringBuilder.ToString(), false);

            // Add all extra properties.
            embedBuilder.WithColor(Color.Green)
                .WithCurrentTimestamp()
                // Remove all the HTML elements from the description.
                .WithDescription($"_{Regex.Replace(media.description, "(<\\/?\\w+>)", " ")}_")
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
                stringBuilder.Append("\n**Anime Statistics**\n");
                stringBuilder.Append($"`Total Entries` {user.statistics.anime.count.ToString("N0", CultureInfo.InvariantCulture)}\n");
                stringBuilder.Append($"`Episodes Watched` {user.statistics.anime.episodesWatched.ToString("N0", CultureInfo.InvariantCulture)}\n");
                TimeSpan t = TimeSpan.FromMinutes(user.statistics.anime.minutesWatched);
                stringBuilder.Append($"`Time Watched` {string.Format("{0:00} Days - {1:00} Hours - {2:00} Minutes", t.Days, t.Hours, t.Minutes)}\n");
                stringBuilder.Append($"`Mean Score` {user.statistics.anime.meanScore.ToString("N2", CultureInfo.InvariantCulture)}\n");
            }

            if (withManga)
            {
                stringBuilder.Append("\n**Manga Statistics**\n");
                stringBuilder.Append($"`Total Entries` {user.statistics.manga.count.ToString("N0", CultureInfo.InvariantCulture)}\n");
                stringBuilder.Append($"`Volumes Read` {user.statistics.manga.volumesRead.ToString("N0", CultureInfo.InvariantCulture)}\n");
                stringBuilder.Append($"`Chapters Read` {user.statistics.manga.chaptersRead.ToString("N0", CultureInfo.InvariantCulture)}\n");
                stringBuilder.Append($"`Mean Score` {user.statistics.manga.meanScore.ToString("N2", CultureInfo.InvariantCulture)}\n");
            }
            
            embedBuilder.WithDescription(stringBuilder.ToString());

            // Add all extra properties.
            embedBuilder.WithColor(Color.DarkPurple)
                .WithCurrentTimestamp()
                .WithImageUrl(user.bannerImage)
                .WithThumbnailUrl(user.avatar.large)
                .WithTitle(user.name)
                .WithUrl(user.siteUrl);

            return embedBuilder.Build();
        }
    }
}

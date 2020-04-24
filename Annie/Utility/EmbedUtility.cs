using AnnieMayDiscordBot.Enums;
using AnnieMayDiscordBot.Models;
using Discord;
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
    }
}

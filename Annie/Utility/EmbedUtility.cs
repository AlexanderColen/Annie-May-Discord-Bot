using AnnieMayDiscordBot.Enums;
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
        /// <summary>
        /// Build the Discord embed for an Anilist Media entry.
        /// </summary>
        /// <param name="media">The Anilist Media object.</param>
        /// <param name="embedMediaList">A List with all the Users and their scores for this Media entry.</param>
        /// <returns>The Discord.NET Embed object.</returns>
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
                            // Display a ? if no score. (0 indicates no score on Anilist)
                            if (embedMedia.score == 0)
                            {
                                completedStringBuilder.Append($"{embedMedia.discordName} **?** | ");
                            }
                            // Display the score otherwise.
                            else
                            {
                                completedStringBuilder.Append($"{embedMedia.discordName} **{embedMedia.score}** | ");
                            }
                            break;
                        case EmbedMediaListStatus.CURRENT:
                            inProgressStringBuilder.Append($"{embedMedia.discordName} [{embedMedia.progress}] | ");
                            break;
                        case EmbedMediaListStatus.DROPPED:
                            droppedStringBuilder.Append($"{embedMedia.discordName} [{embedMedia.progress}] | ");
                            break;
                        case EmbedMediaListStatus.PAUSED:
                            inProgressStringBuilder.Append($"{embedMedia.discordName} [{embedMedia.progress}] | ");
                            break;
                        case EmbedMediaListStatus.PLANNING:
                            plannedStringBuilder.Append($"{embedMedia.discordName} | ");
                            break;
                        default:
                            notOnListStringBuilder.Append($"{embedMedia.discordName} | ");
                            break;
                    }
                }

                string inProgress = inProgressStringBuilder.ToString().TrimEnd(' ', '|');
                string completed = completedStringBuilder.ToString().TrimEnd(' ', '|');
                string dropped = droppedStringBuilder.ToString().TrimEnd(' ', '|');
                string planned = plannedStringBuilder.ToString().TrimEnd(' ', '|');
                string notOnList = notOnListStringBuilder.ToString().TrimEnd(' ', '|');

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
                .WithThumbnailUrl(media.coverImage.extraLarge)
                .WithUrl(media.siteUrl);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Build the Discord embed for an Anilist Character entry.
        /// </summary>
        /// <param name="character">The Anilist Character object.</param>
        /// <returns>The Discord.NET Embed object.</returns>
        public Embed BuildAnilistCharacterEmbed(Character character, bool includeSpoilers = false)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            if (character.description != null)
            {
                string descriptionSpoilerFree = "";
                // Reformat spoilers if requested.
                if (includeSpoilers)
                {
                    // Split the description at Anilist spoilers. (These use ~!I am a spoiler.!~ for formatting)
                    List<string> descriptionParts = character.description.Split("~!").ToList();
                    List<string> formattedParts = new List<string>();

                    // Loop through all parts to format spoilers the Markdown way.
                    foreach (string part in descriptionParts)
                    {
                        // It is a spoiler if the end is still there.
                        if (part.TrimEnd().EndsWith("!~"))
                        {
                            formattedParts.Add($"||{part.Replace("!~", "||")}");
                        }
                        // Otherwise it is not a spoiler.
                        else
                        {
                            formattedParts.Add(part);
                        }
                    }

                    descriptionSpoilerFree = string.Join("\n\n", formattedParts);
                }
                // Otherwise only take the parts before spoilers.
                else
                {
                    descriptionSpoilerFree = character.description.Split("~!")[0];
                }

                embedBuilder.WithDescription($"_{descriptionSpoilerFree}_");
            }
            
            StringBuilder stringBuilderAnime = new StringBuilder();
            StringBuilder stringBuilderManga = new StringBuilder();
            // Zip the nodes and edges to corresponse the media to the roles that a character played.
            foreach (var nodeEdge in character.media.nodes.Zip(character.media.edges, (n, e) => new {node = n, edge = e}))
            {
                string mediaTitle = nodeEdge.node.title.english ?? nodeEdge.node.title.romaji;

                // Add to Anime specific stringbuilder.
                if (nodeEdge.node.type.Equals(MediaType.ANIME))
                {
                    stringBuilderAnime.Append($"• `{mediaTitle}` _[{nodeEdge.edge.characterRole}]_\n");
                }
                // Add to Manga specific stringbuilder.
                else if (nodeEdge.node.type.Equals(MediaType.MANGA))
                {
                    stringBuilderManga.Append($"• `{mediaTitle}` _[{nodeEdge.edge.characterRole}]_\n");
                }
            }

            // Add the appearances fields. Add 'None' if they are empty.
            if (stringBuilderAnime.Length != 0)
            {
                embedBuilder.AddField("Anime Appearances", stringBuilderAnime.ToString());
            }
            else
            {
                embedBuilder.AddField("Anime Appearances", "None");
            }
            if (stringBuilderManga.Length != 0)
            {
                embedBuilder.AddField("Manga Appearances", stringBuilderManga.ToString());
            }
            else
            {
                embedBuilder.AddField("Manga Appearances", "None");
            }

            // Add ID.
            embedBuilder.AddField("ID", character.id);

            // Add name aliases.
            StringBuilder stringBuilderName = new StringBuilder();

            if (character.name.full != null)
            {
                stringBuilderName.Append($"`{character.name.full}` ~ ");
            }

            if (character.name.native != null)
            {
                stringBuilderName.Append($"`{character.name.native}` ~ ");
            }
            // Including all the alternative names, if they are included.
            if (character.name.alternative != null)
            {
                foreach (string altName in character.name.alternative)
                {
                    // Check for non-empty alternative names. (Because for some reason those exist...)
                    if (altName.Length > 0)
                    {
                        stringBuilderName.Append($"`{altName}` ~ ");
                    }
                }
            }
            embedBuilder.AddField("Aliases", stringBuilderName.ToString().TrimEnd(' ', '~'));

            // Add amount of time favourited.
            embedBuilder.AddField("Favourites", character.favourites);

            // Add all extra properties.
            embedBuilder.WithColor(Color.DarkPurple)
                .WithThumbnailUrl(character.image.large)
                .WithTitle($"{character.name.full} ({character.name.native})")
                .WithUrl(character.siteUrl);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Build the Discord embed for an Anilist User entry.
        /// </summary>
        /// <param name="user">The Anilist User.</param>
        /// <param name="withAnime">Boolean indicating whether Anime should be included. Default: true</param>
        /// <param name="withManga">Boolean indicating whether Manga should be included. Default: true</param>
        /// <returns>The Discord.NET Embed object.</returns>
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
                    stringBuilder.Append($"_Total Entries:_ {user.statistics.manga.count.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"_Volumes Read:_ {user.statistics.manga.volumesRead.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"_Chapters Read:_ {user.statistics.manga.chaptersRead.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"_Mean Score:_ {user.statistics.manga.meanScore.ToString("N2", CultureInfo.InvariantCulture)}\n");
                }
            }
            
            embedBuilder.WithDescription(stringBuilder.ToString());

            // Add all extra properties.
            embedBuilder.WithColor(Color.DarkPurple)
                .WithImageUrl(user.bannerImage)
                .WithThumbnailUrl(user.avatar.large)
                .WithTitle($"{user.name} AniList Statistics")
                .WithUrl(user.siteUrl);

            return embedBuilder.Build();
        }
    }
}

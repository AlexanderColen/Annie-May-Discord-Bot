using AnnieMayDiscordBot.Enums;
using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.Models.GitHub;
using Discord;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace AnnieMayDiscordBot.Utility
{
    public class EmbedUtility
    {
        private static readonly int FIELD_LIMIT = 1024;
        private static readonly int DESCRIPTION_LIMIT = 2048;

        /// <summary>
        /// Build the Discord embed with information about this Discord bot from GitHub.
        /// </summary>
        /// <param name="gitHubRepository">The GitHub repository object</param>
        /// <returns>The Discord.NET Embed object.</returns>
        public Embed BuildAboutEmbed(Repository repository)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            // Start building the description.
            StringBuilder stringBuilder = new StringBuilder();
            
            stringBuilder.Append("今日は！私はアニー・メイです！\n");
            stringBuilder.Append("日本語を話せますか？\n");
            stringBuilder.Append("*ahem* I guess I will introduce myself again...\n\n");

            stringBuilder.Append("Hi there! Annie May is here, at your service!\n");
            stringBuilder.Append("I am a Discord bot written by <@!209076181365030913>.\n\n");
            stringBuilder.Append("My expertise lies in time manipulation, killing and seducing Shidou.\n");
            stringBuilder.Append("...I guess I am also quite good at looking things up on **Anilist** if you need some assistance. \uD83D\uDE1C\n\n");
            
            stringBuilder.Append($"If you need my help, just yell `{Properties.Resources.PREFIX}help` and I will appear before you!\n\n");

            stringBuilder.Append($"_Just make sure that you have gone through `{Properties.Resources.PREFIX}setup` to be able to make full use of my prowess!_");

            embedBuilder.WithDescription(stringBuilder.ToString());
            
            embedBuilder.AddField("Version", $"{Properties.Resources.VERSION_MAJOR}.{Properties.Resources.VERSION_MINOR}", true);
            embedBuilder.AddField("Language", $"{repository.PrimaryLanguage.Name}", true);
            embedBuilder.AddField("Framework", $"[Discord.NET](https://discord.foxbot.me/docs/index.html)", true);
            embedBuilder.AddField("Created", $"{repository.CreatedAt.ToShortDateString()}", true);
            embedBuilder.AddField("Last Update", $"{repository.PushedAt.ToShortDateString()}", true);
            
            // Add all extra properties.
            embedBuilder.WithColor(Color.DarkPurple)
                .WithFooter("Feel free to help out with my development at my GitHub page. (Click on the title!)", "https://github.githubassets.com/images/modules/logos_page/GitHub-Mark.png")
                .WithTitle("Who is Annie May?")
                .WithThumbnailUrl("https://vignette.wikia.nocookie.net/date-a-live/images/5/59/DAL_Kurumi_profile.png/revision/latest?cb=20140505161820")
                .WithUrl(repository.Url.AbsoluteUri);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Build the Discord embed for an Anilist Media entry.
        /// </summary>
        /// <param name="media">The Anilist Media object.</param>
        /// <param name="embedMediaList">A List with all the Users and their scores for this Media entry.</param>
        /// <returns>The Discord.NET Embed object.</returns>
        public Embed BuildAnilistMediaEmbed(Media media, List<EmbedMedia> embedMediaList)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            string season = media.Season != null ? media.Season.ToString() : "?";
            string seasonYear = media.SeasonYear != null ? media.SeasonYear.ToString() : "?";

            // First row.
            embedBuilder.AddField("**Type**", media.Type, true)
                        .AddField("**Status**", media.Status, true);

            // Check if both season and seasonYear are empty, if so replace both with just one question mark.
            if (season.Equals("?") && seasonYear.Equals("?"))
            {
                embedBuilder.AddField("**Season**", "?", true);
            }
            else
            {
                embedBuilder.AddField("**Season**", $"{season} {seasonYear}", true);
            }

            // Second row.
            embedBuilder.AddField("**Anilist Score**", media.MeanScore != null ? $"{media.MeanScore}/100" : "-", true)
                .AddField("**Popularity**", media.Popularity, true)
                .AddField("**Favourited**", $"{media.Favourites} times", true);

            // Third row differs for anime and manga.
            if (media.Type == MediaType.Anime)
            {
                embedBuilder.AddField("**Episodes**", media.Episodes != null ? $"{media.Episodes}" : "?", true)
                    .AddField("**Duration**", media.Duration != null ? $"{media.Duration} minutes per episode" : "?", true);
            }
            else if (media.Type == MediaType.Manga)
            {
                embedBuilder.AddField("**Volumes**", media.Volumes != null ? $"{media.Volumes}" : "?", true)
                    .AddField("**Chapters**", media.Chapters != null ? $"{media.Chapters}" : "?", true);
            }

            // Fourth row.
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("`");
            stringBuilder.Append(string.Join("` - `", media.Genres));
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
                        case EmbedMediaListStatus.Completed:
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

                        case EmbedMediaListStatus.Current:
                            inProgressStringBuilder.Append($"{embedMedia.discordName} [{embedMedia.progress}] | ");
                            break;

                        case EmbedMediaListStatus.Dropped:
                            droppedStringBuilder.Append($"{embedMedia.discordName} [{embedMedia.progress}] | ");
                            break;

                        case EmbedMediaListStatus.Paused:
                            inProgressStringBuilder.Append($"{embedMedia.discordName} [{embedMedia.progress}] | ");
                            break;

                        case EmbedMediaListStatus.Planning:
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

                // Don't add empty builders.
                if (inProgress.Length != 0)
                {
                    stringBuilder.Append($"**In-Progress**: {inProgress}\n");
                }

                if (completed.Length != 0)
                {
                    stringBuilder.Append($"**Completed**: {completed}\n");
                }

                if (dropped.Length != 0)
                {
                    stringBuilder.Append($"**Dropped**: {dropped}\n");
                }

                if (planned.Length != 0)
                {
                    stringBuilder.Append($"**Planned**: {planned}\n");
                }

                if (notOnList.Length != 0)
                {
                    stringBuilder.Append($"**Not-On-List**: {notOnList}\n");
                }

                // Also don't add an empty user scores field.
                if (stringBuilder.Length != 0)
                {
                    embedBuilder.AddField("**User Scores**", stringBuilder.ToString());
                }
            }

            // Only do this if the media contains a description.
            if (!string.IsNullOrEmpty(media.Description))
            {
                // Cleanse description of non-escaped HTML tags.
                string description = HttpUtility.HtmlDecode(media.Description);

                // Remove all the HTML elements from the description.
                description = $"[MyAnimeList Alternative](https://myanimelist.net/anime/{media.IdMal})\n\n_{Regex.Replace(description, "(<\\/?\\w+>)", " ")}_";

                // Cut string if necessary.
                if (description.Length > DESCRIPTION_LIMIT)
                {
                    description = CutStringWithEllipsis(description);
                }

                embedBuilder.WithDescription(description);
            }

            // Add all extra properties.
            embedBuilder.WithColor(Color.DarkPurple)
                .WithTitle(media.Title.English ?? media.Title.Romaji)
                .WithThumbnailUrl(media.CoverImage.ExtraLarge)
                .WithUrl(media.SiteUrl);

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

            if (!string.IsNullOrEmpty(character.Description))
            {
                // Cleanse description of non-escaped HTML tags.
                character.Description = HttpUtility.HtmlDecode(character.Description);

                string descriptionSpoilerFree = "";
                // Reformat spoilers if requested.
                if (includeSpoilers)
                {
                    // Split the description at Anilist spoilers. (These use ~!I am a spoiler.!~ for formatting)
                    List<string> descriptionParts = character.Description.Split("~!").ToList();
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
                    descriptionSpoilerFree = character.Description.Split("~!")[0];
                }

                // Cut off string if it goes over the limit.
                descriptionSpoilerFree = CutStringWithEllipsis($"_{descriptionSpoilerFree}_");

                embedBuilder.WithDescription(descriptionSpoilerFree);
            }

            // If spoilers are to be included, it means the rest should be omitted.
            if (!includeSpoilers)
            {
                StringBuilder stringBuilderAnime = new StringBuilder();
                StringBuilder stringBuilderManga = new StringBuilder();
                bool animeLimit = false;
                bool mangaLimit = false;
                int cutAnime = 0;
                int cutManga = 0;
                // Zip the nodes and edges to correspond the media to the roles that a character played.
                foreach (var nodeEdge in character.Media.Nodes.Zip(character.Media.Edges, (n, e) => new { node = n, edge = e }))
                {
                    string mediaTitle = nodeEdge.node.Title.English ?? nodeEdge.node.Title.Romaji;
                    // Add to Anime specific stringbuilder.
                    if (nodeEdge.node.Type.Equals(MediaType.Anime))
                    {
                        string animeAppendage = $"• [{mediaTitle}]({nodeEdge.node.SiteUrl}) _[{nodeEdge.edge.CharacterRole}]_\n";
                        // Only add if there is enough room left.
                        if (stringBuilderAnime.Length + animeAppendage.Length <= FIELD_LIMIT)
                        {
                            stringBuilderAnime.Append(animeAppendage);
                        }
                        else
                        {
                            cutAnime += 1;
                            animeLimit = true;
                        }
                    }
                    // Add to Manga specific stringbuilder.
                    else if (nodeEdge.node.Type.Equals(MediaType.Manga))
                    {
                        string mangaAppendage = $"• [{mediaTitle}]({nodeEdge.node.SiteUrl}) _[{nodeEdge.edge.CharacterRole}]_\n";
                        // Only add if there is enough room left.
                        if (stringBuilderManga.Length + mangaAppendage.Length <= FIELD_LIMIT)
                        {
                            stringBuilderManga.Append(mangaAppendage);
                        }
                        else
                        {
                            cutManga += 1;
                            mangaLimit = true;
                        }
                    }
                }

                // Add the appearances fields. Omit if they are empty.
                if (stringBuilderAnime.Length != 0)
                {
                    // Append amount of cut anime if there is space.
                    if (animeLimit && stringBuilderAnime.Length + 3 + cutAnime.ToString().Length <= FIELD_LIMIT)
                    {
                        stringBuilderAnime.Append($"_+{cutAnime}_");
                    }

                    embedBuilder.AddField("Anime Appearances", stringBuilderAnime.ToString());
                }

                if (stringBuilderManga.Length != 0)
                {
                    // Append amount of cut manga if there is space.
                    if (mangaLimit && stringBuilderManga.Length + 3 + cutManga.ToString().Length <= FIELD_LIMIT)
                    {
                        stringBuilderManga.Append($"_+{cutManga}_");
                    }

                    embedBuilder.AddField("Manga Appearances", stringBuilderManga.ToString());
                }

                // Add name aliases.
                embedBuilder.AddField("Aliases", FormatNameAliases(character.Name));

                // Add ID.
                embedBuilder.AddField("Anilist ID", character.Id, true);

                // Add amount of time favourited.
                embedBuilder.AddField("Favourites", character.Favourites, true);
            }

            // Check if native name exists before adding it as the title.
            string title = character.Name.Full;

            if (!string.IsNullOrEmpty(character.Name.Native))
            {
                title += $" ({character.Name.Native})";
            }

            // Add all extra properties.
            embedBuilder.WithColor(Color.DarkPurple)
                .WithFooter("Description and appearances may be cut off because of character or Anilist limits.")
                .WithThumbnailUrl(character.Image.Large)
                .WithTitle(title)
                .WithUrl(character.SiteUrl);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Build the Discord embed for an Anilist Staff entry.
        /// </summary>
        /// <param name="staff">The Anilist Staff object.</param>
        /// <returns>The Discord.NET Embed object.</returns>
        public Embed BuildAnilistStaffEmbed(Staff staff)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            if (staff.Characters != null)
            {
                StringBuilder stringBuilderCharacters = new StringBuilder();
                bool characterLimit = false;
                int cutCharacters = 0;
                // Zip the nodes and edges to correspond the staff to the roles that their character played.
                foreach (var nodeEdge in staff.Characters.Nodes.Zip(staff.Characters.Edges, (n, e) => new { node = n, edge = e }))
                {
                    string characterAppendage = $"• [{nodeEdge.node.Name.Full}]({nodeEdge.node.SiteUrl}) _[{nodeEdge.edge.Role}]_\n";
                    // Only add if there is enough room left.
                    if (stringBuilderCharacters.Length + characterAppendage.Length <= FIELD_LIMIT)
                    {
                        stringBuilderCharacters.Append(characterAppendage);
                    }
                    else
                    {
                        cutCharacters += 1;
                        characterLimit = true;
                    }
                }

                // Add the voiced characters field. Omit if it is empty.
                if (stringBuilderCharacters.Length != 0)
                {
                    // Append amount of cut anime if there is space.
                    if (characterLimit && stringBuilderCharacters.Length + 3 + cutCharacters.ToString().Length <= FIELD_LIMIT)
                    {
                        stringBuilderCharacters.Append($"_+{cutCharacters}_");
                    }

                    embedBuilder.AddField("Characters Voiced", stringBuilderCharacters.ToString());
                }
            }

            if (staff.StaffMedia != null)
            {
                StringBuilder stringBuilderStaffMedia = new StringBuilder();
                bool mediaLimit = false;
                int cutMedia = 0;
                // Zip the nodes and edges to correspond the staff to productions they worked on.
                foreach (var nodeEdge in staff.StaffMedia.Nodes.Zip(staff.StaffMedia.Edges, (n, e) => new { node = n, edge = e }))
                {
                    string mediaTitle = nodeEdge.node.Title.English ?? nodeEdge.node.Title.Romaji;
                    string mediaAppendage = $"• [{mediaTitle}]({nodeEdge.node.SiteUrl}) _[{nodeEdge.edge.StaffRole}]_\n";
                    // Only add if there is enough room left.
                    if (stringBuilderStaffMedia.Length + mediaAppendage.Length <= FIELD_LIMIT)
                    {
                        stringBuilderStaffMedia.Append(mediaAppendage);
                    }
                    else
                    {
                        cutMedia += 1;
                        mediaLimit = true;
                    }
                }

                // Add the voiced characters field. Omit if it is empty.
                if (stringBuilderStaffMedia.Length != 0)
                {
                    // Append amount of cut anime if there is space.
                    if (mediaLimit && stringBuilderStaffMedia.Length + 3 + cutMedia.ToString().Length <= FIELD_LIMIT)
                    {
                        stringBuilderStaffMedia.Append($"_+{cutMedia}_");
                    }

                    embedBuilder.AddField("Worked On", stringBuilderStaffMedia.ToString());
                }
            }

            // Add name aliases.
            embedBuilder.AddField("Aliases", FormatNameAliases(staff.Name));

            // Add ID.
            embedBuilder.AddField("Anilist ID", staff.Id, true);

            // Add amount of time favourited.
            embedBuilder.AddField("Favourites", staff.Favourites, true);

            // Add the language.
            embedBuilder.AddField("Language", staff.Language, true);

            // Check if native name exists before adding it as the title.
            string name = staff.Name.Full;

            if (!string.IsNullOrEmpty(staff.Name.Native))
            {
                name += $" ({staff.Name.Native})";
            }

            // Only do this if the media contains a description.
            if (!string.IsNullOrEmpty(staff.Description))
            {
                // Cleanse description of non-escaped HTML tags.
                string description = HttpUtility.HtmlDecode(staff.Description);

                // Remove all the HTML elements from the description.
                description = $"{Regex.Replace(description, "(<\\/?\\w+>)", " ")}";

                // Cut string if necessary.
                if (description.Length > DESCRIPTION_LIMIT)
                {
                    description = CutStringWithEllipsis(description);
                }

                embedBuilder.WithDescription(description);
            }

            // Add all extra properties.
            embedBuilder.WithColor(Color.DarkPurple)
                .WithFooter("Characters voiced may be cut off because of character or Anilist limits.")
                .WithThumbnailUrl(staff.Image.Large)
                .WithTitle(name)
                .WithUrl(staff.SiteUrl);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Build the Discord embed for an Anilist Studio entry.
        /// </summary>
        /// <param name="studio">The Anilist Studio object.</param>
        /// <returns>The Discord.NET Embed object.</returns>
        public Embed BuildAnilistStudioEmbed(Studio studio)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            StringBuilder stringBuilderAnime = new StringBuilder();
            StringBuilder stringBuilderManga = new StringBuilder();

            // Zip the nodes and edges to correspond the media to the roles that the studio contributed to.
            foreach (var nodeEdge in studio.Media.Nodes.Zip(studio.Media.Edges, (n, e) => new { node = n, edge = e }))
            {
                string mediaTitle = nodeEdge.node.Title.English ?? nodeEdge.node.Title.Romaji;

                // Add to Anime specific stringbuilder.
                if (nodeEdge.node.Type.Equals(MediaType.Anime))
                {
                    string mediaLink = $"• [{mediaTitle}]({nodeEdge.node.SiteUrl})";

                    // Check and add them as Main Studio if applicable.
                    if (nodeEdge.edge.IsMainStudio)
                    {
                        mediaLink += " _[Main Studio]_";
                    }

                    stringBuilderAnime.Append($"{mediaLink}\n");
                }
                // Add to Manga specific stringbuilder.
                else if (nodeEdge.node.Type.Equals(MediaType.Manga))
                {
                    string mediaLink = $"• [{mediaTitle}]({nodeEdge.node.SiteUrl})";

                    // Check and add them as Main Studio if applicable.
                    if (nodeEdge.edge.IsMainStudio)
                    {
                        mediaLink += " _[Main Studio]_";
                    }

                    stringBuilderAnime.Append($"{mediaLink}\n");
                }
            }

            // Add the produced anime/manga as description.
            StringBuilder stringBuilderDescription = new StringBuilder();
            if (stringBuilderAnime.Length != 0)
            {
                stringBuilderDescription.Append($"**Anime Produced**\n{stringBuilderAnime.ToString()}");
            }
            if (stringBuilderManga.Length != 0)
            {
                stringBuilderDescription.Append($"**Manga Produced**\n{stringBuilderManga.ToString()}");
            }
            embedBuilder.WithDescription(stringBuilderDescription.ToString());

            // Add ID.
            embedBuilder.AddField("Anilist ID", studio.Id, true);

            // Add amount of time favourited.
            embedBuilder.AddField("Favourites", studio.Favourites, true);

            // Add the kind of studio this is. Either Animation or Other since that is the only Anilist info available.
            embedBuilder.AddField("Type", studio.IsAnimationStudio ? "Animation" : "Other", true);

            // Add all extra properties.
            embedBuilder.WithColor(Color.DarkPurple)
                .WithFooter("Not all the media produced by this studio is included.")
                .WithTitle(studio.Name)
                .WithUrl(studio.SiteUrl);

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

            // Build custom description for displaying anime.
            if (withAnime)
            {
                stringBuilder.Append($"\n[**Anime List**]({user.SiteUrl}{"/animelist"})\n");
                stringBuilder.Append($"_Total Entries:_ {user.Statistics.Anime.Count.ToString("N0", CultureInfo.InvariantCulture)}\n");
                stringBuilder.Append($"_Episodes Watched:_ {user.Statistics.Anime.EpisodesWatched.ToString("N0", CultureInfo.InvariantCulture)}\n");
                TimeSpan t = TimeSpan.FromMinutes(user.Statistics.Anime.MinutesWatched);
                stringBuilder.Append($"_Time Watched:_ {t.Days:00} Days - {t.Hours:00} Hours - {t.Minutes:00} Minutes\n");
                stringBuilder.Append($"_Mean Score:_ {user.Statistics.Anime.MeanScore.ToString("N2", CultureInfo.InvariantCulture)}\n");
            }

            // Build custom description for displaying manga.
            if (withManga)
            {
                stringBuilder.Append($"\n[**Manga List**]({user.SiteUrl}{"/mangalist"})\n");
                // Alternative meme response for creator.
                if (user.Name == "SmellyAlex")
                {
                    stringBuilder.Append("_Total Entries:_ -1\n");
                    stringBuilder.Append("_Volumes Read:_ -1\n");
                    stringBuilder.Append("_Chapters Read:_ -1\n");
                    stringBuilder.Append("_Mean Score:_ -100\n");
                }
                // Otherwise regular with actual numbers.
                else
                {
                    stringBuilder.Append($"_Total Entries:_ {user.Statistics.Manga.Count.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"_Volumes Read:_ {user.Statistics.Manga.VolumesRead.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"_Chapters Read:_ {user.Statistics.Manga.ChaptersRead.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"_Mean Score:_ {user.Statistics.Manga.MeanScore.ToString("N2", CultureInfo.InvariantCulture)}\n");
                }
            }

            embedBuilder.WithDescription(stringBuilder.ToString());

            // Add all extra properties.
            embedBuilder.WithColor(Color.DarkPurple)
                .WithImageUrl(user.BannerImage)
                .WithThumbnailUrl(user.Avatar.Large)
                .WithTitle($"{user.Name} AniList Statistics")
                .WithUrl(user.SiteUrl);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Cut off a string at a certain limit and insert ellipsis (...) to indicate this has been done.
        /// </summary>
        /// <param name="fullString">The string to cut.</param>
        /// <param name="limit">The limit to cut off at. Defaults to 2048.</param>
        /// <returns>The cut off string at the limit ending in ellipsis.</returns>
        private string CutStringWithEllipsis(string fullString, int limit = 2048)
        {
            // Check if the string is already within the bounds.
            if (fullString.Length <= limit)
            {
                return fullString;
            }

            // Cut last three characters.
            string alteredString = fullString.Remove(limit - 3);
            // Pad with ellipsis (...) at the end.
            alteredString += "...";

            // Check for corrupted Markdown spoiler tags, which only happens when an uneven amount is present.
            if (new Regex(Regex.Escape("||")).Matches(alteredString).Count % 2 > 0)
            {
                /**
                 * 5 is the magic number, because we need to remove the added ellipsis (3)
                 * and make space for the closing spoiler tag (2).
                 **/
                alteredString = alteredString.Remove(limit - 5);
                alteredString += "...||";
            }

            return alteredString;
        }

        /// <summary>
        /// Format all the known aliases for an Anilist entry into a Markdown string.
        /// </summary>
        /// <param name="name">The AnilistName of the object. This class is extended.</param>
        /// <returns>Markdown format of all the known aliases of this class.</returns>
        private string FormatNameAliases(AnilistName name)
        {
            List<string> names = new List<string>();

            // Only add full name if it is included.
            if (!string.IsNullOrEmpty(name.Full))
            {
                names.Add($"`{name.Full}`");
            }

            // Only add native name if it is included.
            if (!string.IsNullOrEmpty(name.Native))
            {
                names.Add($"`{name.Native}`");
            }

            // Including all the alternative names, if they are included.
            if (name.Alternative != null)
            {
                foreach (string altName in name.Alternative)
                {
                    /*
                     * Check for null or non-empty alternative names. (Because for some reason those exist...)
                     * Also check for duplicate names, since that occurs a lot.
                     */
                    if (!string.IsNullOrEmpty(altName) && !names.Any(s => s.Equals($"`{altName}`")))
                    {
                        names.Add($"`{altName.TrimEnd()}`");
                    }
                }
            }

            return string.Join(" ~ ", names);
        }
    }
}
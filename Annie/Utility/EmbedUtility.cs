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
            stringBuilder.Append("My expertise lies in time manipulation, killing, and seducing Shidou.\n");
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
            embedBuilder.WithColor(Color.DarkRed)
                .WithFooter("Feel free to help out with my development at my GitHub page. (Click on the title!)", "https://github.githubassets.com/images/modules/logos_page/GitHub-Mark.png")
                .WithTitle("Who is Annie May?")
                .WithThumbnailUrl("https://vignette.wikia.nocookie.net/date-a-live/images/5/59/DAL_Kurumi_profile.png/revision/latest?cb=20140505161820")
                .WithUrl(repository.Url.AbsoluteUri);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Build the Discord embed with calculated Anilist affinity between two users.
        /// </summary>
        /// <param name="affinityDict">A dictionary containing both user's names and the list of shared media.</param>
        /// <returns>The Discord.NET Embed object.</returns>
        public Embed BuildAffinityEmbed(Dictionary<string, object> affinityDict)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            string affinityDescription;

            // Prefetch all the values.
            affinityDict.TryGetValue("shared", out object sharedMedia);
            affinityDict.TryGetValue("userA", out object userA);
            affinityDict.TryGetValue("userB", out object userB);
            affinityDict.TryGetValue("affinity", out object affinity);

            // Don't bother with affinity if there is no shared media.
            if (((List<(int, float, float)>)sharedMedia).Count == 0)
            {
                affinityDescription = $"Could not compute affinity between " +
                    $"**[{((User)userA).Name}]({((User)userA).SiteUrl})** and " +
                    $"**[{((User)userB).Name}]({((User)userB).SiteUrl})**" +
                    " because of the lack of _(scored)_ shared media.";
            }
            else
            {
                string affinityString = "Unknown";

                if ((double)affinity != -404)
                {
                    affinityString = ((double)affinity * 100).ToString("N2", CultureInfo.InvariantCulture);
                }

                affinityDescription = $"**{affinityString}%** affinity between " +
                        $"**[{((User)userA).Name}]({((User)userA).SiteUrl})** and " +
                        $"**[{((User)userB).Name}]({((User)userB).SiteUrl})**.\n" +
                        $"_({((List<(int, float, float)>)sharedMedia).Count} shared scored media entries)_";
            }

            var colour = ConvertStringToDiscordColour(((User)userA).Options.ProfileColor);
            embedBuilder.WithColor(colour.Item1, colour.Item2, colour.Item3)
                .WithDescription(affinityDescription)
                .WithFooter("My affinity calculations aren't perfect. " +
                "If you know how to fix them, please visit my GitHub page.")
                .WithThumbnailUrl(((User)userA).Avatar.Large)
                .WithTitle($"{((User)userA).Name} Affinity")
                .WithUrl(((User)userA).SiteUrl);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Build the Discord embed with calculated Anilist affinity between Guild users.
        /// </summary>
        /// <param name="dicts">An array of Dictionaries containing users and shared media.</param>
        /// <returns>The Discord.NET Embed object.</returns>
        public Embed BuildAffinityListEmbed(List<Dictionary<string, object>> dicts)
        {
            // Sort dictionaries on affinity.
            var sortedDicts = dicts.OrderByDescending(dict => dict["affinity"]).ToList();

            // Prefetch main User.
            sortedDicts[0].TryGetValue("userA", out object userA);

            EmbedBuilder embedBuilder = new EmbedBuilder();

            StringBuilder stringBuilder = new StringBuilder();

            // Loop over all calculated affinities.
            foreach (var dict in sortedDicts)
            {
                // Fetch UserB, shared Media and affinity.
                dict.TryGetValue("userB", out object userB);
                dict.TryGetValue("discordUsername", out object discordUsername);
                dict.TryGetValue("shared", out object sharedMedia);
                dict.TryGetValue("affinity", out object affinity);

                if ((double)affinity == -404)
                {
                    stringBuilder.Append($"**?** with **[{((User)userB).Name}]({((User)userB).SiteUrl})** ({discordUsername}) " +
                    $"\u2043 _[{((List<(int, float, float)>)sharedMedia).Count} shared scored media]_\n");
                }
                else
                {
                    stringBuilder.Append($"**{((double)affinity * 100).ToString("N2", CultureInfo.InvariantCulture)}%** " +
                        $"with **[{((User)userB).Name}]({((User)userB).SiteUrl})** ({discordUsername}) " +
                    $"\u2043 _[{((List<(int, float, float)>)sharedMedia).Count} shared scored media]_\n");
                }
            }

            embedBuilder.WithDescription(CutStringWithEllipsis(stringBuilder.ToString()));

            var colour = ConvertStringToDiscordColour(((User)userA).Options.ProfileColor);
            embedBuilder.WithColor(colour.Item1, colour.Item2, colour.Item3)
                .WithFooter("My affinity calculations aren't perfect. " +
                "If you know how to fix them, please visit my GitHub page.")
                .WithThumbnailUrl(((User)userA).Avatar.Large)
                .WithTitle($"{((User)userA).Name} Affinity")
                .WithUrl(((User)userA).SiteUrl);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Build the Discord embed for an Anilist Media entry.
        /// </summary>
        /// <param name="media">The Anilist Media object.</param>
        /// <param name="embedMediaList">A List with all the Users and their scores for this Media entry.</param>
        /// <param name="showScores">Boolean indicating whether to include User's scores or not.</param>
        /// <returns>The Discord.NET Embed object.</returns>
        public Embed BuildAnilistMediaEmbed(Media media, List<EmbedMedia> embedMediaList = null, bool showScores = false)
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
            embedBuilder.AddField("**Anilist Score**", media.MeanScore != null && media.MeanScore != 0 ? $"{media.MeanScore}/100" : "-", true)
                .AddField("**Popularity**", media.Popularity, true)
                .AddField("**Favourited**", $"{media.Favourites} times", true);

            // Third row differs for anime and manga.
            if (media.Type == MediaType.Anime)
            {
                string duration = media.Duration != null && media.Duration != 0 ? $"{media.Duration} minutes" : "?";

                // Add 'per episode' for TV, OVA, ONA and Specials.
                if (duration != "?" && (media.Format.Equals(MediaFormat.ONA) || media.Format.Equals(MediaFormat.OVA)
                    || media.Format.Equals(MediaFormat.TV) || media.Format.Equals(MediaFormat.Special)))
                {
                    duration += " per episode";
                }

                embedBuilder.AddField("**Episodes**", media.Episodes != null && media.Episodes != 0 ? $"{media.Episodes}" : "?", true)
                    .AddField("**Duration**", duration, true)
                    .AddField("**Format**", media.Format, true);
            }
            else if (media.Type == MediaType.Manga)
            {
                embedBuilder.AddField("**Volumes**", media.Volumes != null && media.Volumes != 0 ? $"{media.Volumes}" : "?", true)
                    .AddField("**Chapters**", media.Chapters != null && media.Chapters != 0 ? $"{media.Chapters}" : "?", true);
            }

            // Fourth row.
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("`");
            stringBuilder.Append(string.Join("` - `", media.Genres));
            stringBuilder.Append("`");
            embedBuilder.AddField("**Genres**", stringBuilder.ToString());

            // Fifth row with User scores.
            if (embedMediaList != null && showScores)
            {
                stringBuilder.Clear();

                StringBuilder completedStringBuilder = new StringBuilder();
                StringBuilder plannedStringBuilder = new StringBuilder();
                StringBuilder inProgressStringBuilder = new StringBuilder();
                StringBuilder droppedStringBuilder = new StringBuilder();
                StringBuilder notOnListStringBuilder = new StringBuilder();
                StringBuilder repeatingStringBuilder = new StringBuilder();
                foreach (EmbedMedia embedMedia in embedMediaList?.OrderBy(s => s.Progress).ThenBy(s => s.DiscordName))
                {
                    switch (embedMedia.Status)
                    {
                        case EmbedMediaListStatus.Completed:
                            // Display a ? if no score. (0 indicates no score on Anilist)
                            if (embedMedia.Score == 0)
                            {
                                completedStringBuilder.Append($"{embedMedia.DiscordName} **?** | ");
                            }
                            // Display the score otherwise.
                            else
                            {
                                completedStringBuilder.Append($"{embedMedia.DiscordName} **{embedMedia.Score}** | ");
                            }
                            break;

                        case EmbedMediaListStatus.Current:
                            inProgressStringBuilder.Append($"{embedMedia.DiscordName} [{embedMedia.Progress}] | ");
                            break;

                        case EmbedMediaListStatus.Dropped:
                            droppedStringBuilder.Append($"{embedMedia.DiscordName} [{embedMedia.Progress}] | ");
                            break;

                        case EmbedMediaListStatus.Paused:
                            inProgressStringBuilder.Append($"{embedMedia.DiscordName} [{embedMedia.Progress}] | ");
                            break;

                        case EmbedMediaListStatus.Planning:
                            plannedStringBuilder.Append($"{embedMedia.DiscordName} | ");
                            break;

                        case EmbedMediaListStatus.Repeating:
                            // Display a ? if no score. (0 indicates no score on Anilist)
                            if (embedMedia.Score == 0)
                            {
                                completedStringBuilder.Append($"{embedMedia.DiscordName} **?** | ");
                            }
                            // Display the score otherwise.
                            else
                            {
                                repeatingStringBuilder.Append($"{embedMedia.DiscordName} [{embedMedia.Progress}] **{embedMedia.Score}** | ");
                            }
                            break;

                        default:
                            notOnListStringBuilder.Append($"{embedMedia.DiscordName} | ");
                            break;
                    }
                }

                string inProgress = inProgressStringBuilder.ToString().TrimEnd(' ', '|');
                string repeating = repeatingStringBuilder.ToString().TrimEnd(' ', '|');
                string completed = completedStringBuilder.ToString().TrimEnd(' ', '|');
                string dropped = droppedStringBuilder.ToString().TrimEnd(' ', '|');
                string planned = plannedStringBuilder.ToString().TrimEnd(' ', '|');
                string notOnList = notOnListStringBuilder.ToString().TrimEnd(' ', '|');

                // Don't add empty builders.
                if (inProgress.Length != 0)
                {
                    stringBuilder.Append($"**In-Progress**: {inProgress}\n");
                }

                if (repeating.Length != 0)
                {
                    stringBuilder.Append($"**Rewatching**: {repeating}\n");
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
                description = $"_{Regex.Replace(description, "(<\\/?\\w+>)", " ")}_";

                // Add MAL alternative hyperlink if an ID was provided.
                if (media.IdMal != null)
                {
                    description = $"[MyAnimeList Alternative](https://myanimelist.net/{media.Type.ToString().ToLower()}/{media.IdMal})\n\n{description}";
                }

                // Cut string if necessary.
                if (description.Length > DESCRIPTION_LIMIT)
                {
                    description = CutStringWithEllipsis(description);
                }

                embedBuilder.WithDescription(description);
            }

            // Add all extra properties.
            embedBuilder.WithColor(Color.DarkPurple)
                .WithTitle(media.Title.Romaji ?? media.Title.English)
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
                .WithFooter("Characters voiced and description may be cut off because of Discord and/or Anilist limits.")
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
                stringBuilderDescription.Append($"**Anime Produced**\n{stringBuilderAnime}");
            }
            if (stringBuilderManga.Length != 0)
            {
                stringBuilderDescription.Append($"**Manga Produced**\n{stringBuilderManga}");
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
        public Embed BuildAnilistUserEmbed(User user, bool withAnime = true, bool withManga = true)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            StringBuilder stringBuilder = new StringBuilder();

            // Build custom description for displaying anime.
            if (withAnime)
            {
                stringBuilder.Append($"\n[**Anime List**]({user.SiteUrl}{"/animelist"})\n");
                stringBuilder.Append($"\u204C _Total Entries:_ {user.Statistics.Anime.Count.ToString("N0", CultureInfo.InvariantCulture)}\n");
                stringBuilder.Append($"\u204C _Episodes Watched:_ {user.Statistics.Anime.EpisodesWatched.ToString("N0", CultureInfo.InvariantCulture)}\n");
                TimeSpan t = TimeSpan.FromMinutes(user.Statistics.Anime.MinutesWatched);
                stringBuilder.Append($"\u204C _Time Watched:_ {t.Days:00} Days - {t.Hours:00} Hours - {t.Minutes:00} Minutes\n");
                stringBuilder.Append($"\u204C _Mean Score:_ {user.Statistics.Anime.MeanScore.ToString("N2", CultureInfo.InvariantCulture)}\n");
            }

            // Build custom description for displaying manga.
            if (withManga)
            {
                stringBuilder.Append($"\n[**Manga List**]({user.SiteUrl}{"/mangalist"})\n");
                // Alternative meme response for creator.
                if (user.Name == "SmellyAlex")
                {
                    stringBuilder.Append("\u204D _Total Entries:_ -1\n");
                    stringBuilder.Append("\u204D _Volumes Read:_ -1\n");
                    stringBuilder.Append("\u204D _Chapters Read:_ -1\n");
                    stringBuilder.Append("\u204D _Mean Score:_ -100\n");
                }
                // Otherwise regular with actual numbers.
                else
                {
                    stringBuilder.Append($"\u204D _Total Entries:_ {user.Statistics.Manga.Count.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"\u204D _Volumes Read:_ {user.Statistics.Manga.VolumesRead.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"\u204D _Chapters Read:_ {user.Statistics.Manga.ChaptersRead.ToString("N0", CultureInfo.InvariantCulture)}\n");
                    stringBuilder.Append($"\u204D _Mean Score:_ {user.Statistics.Manga.MeanScore.ToString("N2", CultureInfo.InvariantCulture)}\n");
                }
            }

            // Find all genres.
            var allGenres = new List<UserGenreStatistic>();
            allGenres.AddRange(user.Statistics.Anime.Genres);
            if (allGenres.Count == 0)
            {
                allGenres = user.Statistics.Manga.Genres;
            }
            else
            {
                foreach (var genre in user.Statistics.Manga.Genres)
                {
                    bool genreExists = false;
                    // Loop over all genres so far.
                    foreach (var allGenre in allGenres)
                    {
                        // If the genre already exists, add the count to it and recalculate mean score.
                        if (allGenre.Genre == genre.Genre)
                        {
                            genreExists = true;
                            // Calculate total score before adding the count.
                            var total = allGenre.MeanScore * allGenre.Count + genre.MeanScore * genre.Count;
                            allGenre.Count += genre.Count;
                            // Recalculate the mean score.
                            allGenre.MeanScore = total / allGenre.Count;
                            break;
                        }
                    }

                    // If genre did not exist, add it to the list.
                    if (!genreExists)
                    {
                        allGenres.Add(genre);
                    }
                }
            }

            // Find all release years.
            var allReleaseYears = new List<UserReleaseYearStatistic>();
            allReleaseYears.AddRange(user.Statistics.Anime.ReleaseYears);
            if (allReleaseYears.Count == 0)
            {
                allReleaseYears = user.Statistics.Manga.ReleaseYears;
            }
            else
            {
                foreach (var releaseYear in user.Statistics.Manga.ReleaseYears)
                {
                    bool releaseYearExists = false;
                    // Loop over all release years so far.
                    foreach (var allReleaseYear in allReleaseYears)
                    {
                        // If the release year already exists, add the count to it.
                        if (allReleaseYear.ReleaseYear == allReleaseYear.ReleaseYear)
                        {
                            releaseYearExists = true;
                            allReleaseYear.Count += releaseYear.Count;
                            break;
                        }
                    }

                    // If release year did not exist, add it to the list.
                    if (!releaseYearExists)
                    {
                        allReleaseYears.Add(releaseYear);
                    }
                }
            }

            // Find all start years.
            var allStartYears = new List<UserStartYearStatistic>();
            allStartYears.AddRange(user.Statistics.Anime.StartYears);
            if (allStartYears.Count == 0)
            {
                allStartYears = user.Statistics.Manga.StartYears;
            }
            else
            {
                foreach (var startYear in user.Statistics.Manga.StartYears)
                {
                    bool startYearExists = false;
                    // Loop over all release years so far.
                    foreach (var allStartYear in allStartYears)
                    {
                        // If the release year already exists, add the count to it.
                        if (allStartYear.StartYear == allStartYear.StartYear)
                        {
                            startYearExists = true;
                            allStartYear.Count += startYear.Count;
                            break;
                        }
                    }

                    // If release year did not exist, add it to the list.
                    if (!startYearExists)
                    {
                        allStartYears.Add(startYear);
                    }
                }
            }

            // Find all statuses.
            var allStatuses = new List<UserStatusStatistic>();
            allStatuses.AddRange(user.Statistics.Anime.Statuses);
            if (allStatuses.Count == 0)
            {
                allStatuses = user.Statistics.Manga.Statuses;
            }
            else
            {
                foreach (var status in user.Statistics.Manga.Statuses)
                {
                    bool statusExists = false;
                    // Loop over all statuses so far.
                    foreach (var allStatus in allStatuses)
                    {
                        // If the status already exists, add the count to it.
                        if (allStatus.Status == allStatus.Status)
                        {
                            statusExists = true;
                            allStatus.Count += status.Count;
                            break;
                        }
                    }

                    // If status did not exist, add it to the list.
                    if (!statusExists)
                    {
                        allStatuses.Add(status);
                    }
                }
            }

            // Don't add the Weeb Tendencies part if none of the lists contain any elements.
            if (allGenres.Count > 0 || allReleaseYears.Count > 0 || allStartYears.Count > 0 || user.Statistics.Anime.Formats.Count > 0)
            {
                stringBuilder.Append($"\n[**Weeb Tendencies**]({user.SiteUrl}/stats/anime/overview)");
                // Only add genres if more than 3 are available.
                if (allGenres.Count >= 3)
                {
                    // Sort genres on count.
                    allGenres = allGenres.OrderByDescending(o => o.Count).ToList();
                    // Add favourite genres to the stringbuilder.
                    stringBuilder.Append($"\n\u2023 Is a **{allGenres[0].Genre}/{allGenres[1].Genre}/{allGenres[2].Genre}** normie");
                    // Sort genres on score.
                    allGenres = allGenres.OrderBy(o => o.MeanScore).ToList();
                    // Add worst rated genre.
                    stringBuilder.Append($"\n\u2023 Seems to hate **{allGenres[0].Genre}**");
                }

                if (allReleaseYears.Count > 0)
                {
                    // Sort release years on count.
                    allReleaseYears = allReleaseYears.OrderByDescending(o => o.Count).ToList();
                    // Add most common release year to the stringbuilder.
                    stringBuilder.Append($"\n\u2023 Loves **{allReleaseYears[0].ReleaseYear}** media");
                }

                if (allStartYears.Count > 0)
                {
                    // Sort start years on year.
                    allStartYears = allStartYears.OrderBy(o => o.StartYear).ToList();
                    // Add most common start year to the stringbuilder.
                    stringBuilder.Append($"\n\u2023 Started consuming weebness in **{allStartYears[0].StartYear}**");
                }

                if (user.Statistics.Anime.Formats.Count > 0)
                {
                    // Sort formats on count.
                    user.Statistics.Anime.Formats = user.Statistics.Anime.Formats.OrderByDescending(o => o.Count).ToList();
                    // Add most common release year to the stringbuilder.
                    stringBuilder.Append($"\n\u2023 Addicted to the **{user.Statistics.Anime.Formats[0].Format}** format");
                }

                if (allStatuses.Count > 0)
                {
                    // Sort statuses on count.
                    allStatuses = allStatuses.OrderByDescending(o => o.Count).ToList();

                    // Calculate completed %.
                    var total = 0.0;
                    var completed = 0.0;
                    foreach (var status in allStatuses)
                    {
                        // Completed and Repeating media counts as completed.
                        if (status.Status == MediaListStatus.Completed || status.Status == MediaListStatus.Repeating)
                        {
                            completed += status.Count;
                        }

                        // Any media that isn't in Planning counts for the total.
                        if (status.Status != MediaListStatus.Planning)
                        {
                            total += status.Count;
                        }

                        // Special mention for never dropping.
                        if (status.Status == MediaListStatus.Dropped && status.Count == 0)
                        {
                            stringBuilder.Append("\n\u2023 Has **never** dropped an anime/manga!");
                        }
                    }

                    var completedRatio = completed / total * 100;
                    // Add completed-dropped ratio to the stringbuilder.
                    stringBuilder.Append($"\n\u2023 Ends up completing **~{completedRatio:N0}%**");

                    // If plan to watch is the highest, shame them.
                    if (allStatuses[0].Status == MediaListStatus.Planning)
                    {
                        stringBuilder.Append("\n\u2023 Apparently thinks PLANNING > WATCHING...");
                    }
                }

                embedBuilder.WithFooter("Weeb tendencies could be wrong since they are based on Anilist user data...");
            }
            

            embedBuilder.WithDescription(stringBuilder.ToString());
            
            // Add all extra properties.
            var colour = ConvertStringToDiscordColour(user.Options.ProfileColor);
            embedBuilder.WithColor(colour.Item1, colour.Item2, colour.Item3)
                .WithImageUrl(user.BannerImage)
                .WithThumbnailUrl(user.Avatar.Large)
                .WithTitle($"{user.Name} AniList Statistics")
                .WithUrl(user.SiteUrl);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Build the Discord embed for scores from an Anilist MediaListCollection entry.
        /// </summary>
        /// <param name="mediaListCollection">The Anilist MediaListCollection.</param>
        /// <param name="mediaType">The MediaType the scores are for.</param>
        /// <returns>The Discord.NET Embed object.</returns>
        public Embed BuildScoresEmbed(MediaListCollection mediaListCollection, MediaType mediaType)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            var formattedScores = new List<string>();
            var userScoresList = new List<UserScores>();
            // Populate the list of UserScores.
            for (int i = 10; i > 0; i--)
            {
                var userScores = new UserScores
                {
                    UpperBound = 10 * i,
                    LowerBound = 10 * (i - 1) + 1
                };
                userScoresList.Add(userScores);
            }

            int unscored = 0;
            if (mediaListCollection.Lists.Count > 0)
            {
                // Read all the list values.
                foreach (var entry in mediaListCollection.Lists[0].Entries)
                {
                    // Track unscores entries.
                    if (entry.Score == 0)
                    {
                        unscored++;
                    }
                    // Otherwise go look for the current UserScores in the list.
                    else
                    {
                        // Check for SAO
                        bool isSao = entry.Media.Title.English != null && entry.Media.Title.English.ToLower().Contains("sword art online");

                        foreach (var userScores in userScoresList)
                        {
                            // Check if the score falls within the two score bounds.
                            if (userScores.UpperBound >= entry.Score && entry.Score >= userScores.LowerBound)
                            {
                                // Increment count.
                                userScores.Count++;
                                // Check for SAO.
                                if (isSao)
                                {
                                    userScores.HasSAO = true;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            // Format the scores.
            foreach (var userScores in userScoresList)
            {
                var items = 0;
                if (mediaListCollection.Lists.Count > 0)
                {
                    items = mediaListCollection.Lists[0].Entries.Count;
                }
                float percentage = 0;
                if (items != 0)
                {
                    percentage = userScores.Count / items * 100;
                }
                string formatted = $"_{userScores.LowerBound}-{userScores.UpperBound}_: **{userScores.Count}x**" +
                    $" ~ _{percentage.ToString("N2", CultureInfo.InvariantCulture)}%_";
                
                // Check for and add sao score.
                if (userScores.HasSAO)
                {
                    formatted += " _<- SAO_";
                }
                formattedScores.Add(formatted);
            }

            // Custom fun override for specific user.
            if (mediaListCollection.User.Id == 210768 && mediaType.Equals(MediaType.Manga))
            {
                formattedScores.Add($"_-100_: **{int.MaxValue}x** ~ _100%_");
            }

            // Add unscored.
            formattedScores.Add($"_No Score: {unscored}_");
            
            embedBuilder.WithDescription($"**{mediaType} Scores**\n\n{string.Join("\n", formattedScores)}");

            // Add all extra properties.
            var colour = ConvertStringToDiscordColour(mediaListCollection.User.Options.ProfileColor);
            embedBuilder.WithColor(colour.Item1, colour.Item2, colour.Item3)
                .WithThumbnailUrl(mediaListCollection.User.Avatar.Large)
                .WithTitle(mediaListCollection.User.Name)
                .WithUrl(mediaListCollection.User.SiteUrl);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Build the Discord embed for media entries within bounds from an Anilist MediaListCollection entry.
        /// </summary>
        /// <param name="mediaListCollection">The Anilist MediaListCollection.</param>
        /// <param name="mediaType">The MediaType the scores are for.</param>
        /// <param name="min">The lowest score allowed.</param>
        /// <param name="max">The highest score allowed.</param>
        /// <returns>The Discord.NET Embed object.</returns>
        public Embed BuildCustomScoresEmbed(MediaListCollection mediaListCollection, MediaType mediaType, int min, int max)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            // Populate list of UserScores.
            var userScoresList = new List<UserScores>();
            for (int i = 10; i > 0; i--)
            {
                var userScores = new UserScores
                {
                    UpperBound = 10 * i,
                    LowerBound = 10 * (i - 1) + 1,
                    MediaTitles = new List<string>()
                };
                userScoresList.Add(userScores);
            }

            if (mediaListCollection.Lists.Count > 0)
            {
                // Loop over the media entries to find within bounds.
                foreach (var entry in mediaListCollection.Lists[0].Entries)
                {
                    // Only handle entry if it falls within bounds.
                    if (entry.Score >= min && entry.Score <= max)
                    {
                        // Find the corresponding UserScores to add it to the list.
                        foreach (var userScores in userScoresList)
                        {
                            // Check if the score falls within the two score bounds.
                            if (userScores.UpperBound >= entry.Score && entry.Score >= userScores.LowerBound)
                            {
                                userScores.MediaTitles.Add(entry.Media.Title.English ?? entry.Media.Title.Romaji);
                                break;
                            }
                        }
                    }
                }
            }
            
            // Start formatting the description.
            string description = $"**{mediaType} scored between {min} and {max}**\n\n";
            int totalMedia = 0;
            foreach (var userScores in userScoresList)
            {
                // Skip to the next list item if it doesn't fall within the bounds.
                if (userScores.LowerBound < min && userScores.UpperBound > max)
                {
                    continue;
                }
                
                // Make sure to skip empty lists.
                if (userScores.MediaTitles.Count > 0)
                {
                    totalMedia += userScores.MediaTitles.Count;
                    description += $"**{userScores.LowerBound}-{userScores.UpperBound}**\n";
                    description += $"{string.Join("\n", userScores.MediaTitles)}\n\n";
                }

                // If limit has been reached on description, cut it off with an ellipsis and break out of the loop.
                if (description.Length > DESCRIPTION_LIMIT)
                {
                    description = CutStringWithEllipsis(description);
                    break;
                }
            }
            embedBuilder.WithDescription(description);

            // Override description if no media was added.
            if (totalMedia == 0)
            {
                embedBuilder.WithDescription(description + "No entries found for these bounds.");
            }

            // Add all extra properties.
            var colour = ConvertStringToDiscordColour(mediaListCollection.User.Options.ProfileColor);
            embedBuilder.WithColor(colour.Item1, colour.Item2, colour.Item3)
                .WithFooter("Some entries may be hidden because of the Anilist and/or Discord limit...")
                .WithThumbnailUrl(mediaListCollection.User.Avatar.Large)
                .WithTitle(mediaListCollection.User.Name)
                .WithUrl(mediaListCollection.User.SiteUrl);

            return embedBuilder.Build();
        }

        /// <summary>
        /// Build the Discord embed for displaying all the Anilist users in the server.
        /// </summary>
        /// <param name="discordUsers">The list of Discord users.</param>
        /// <param name="guild">The Discord guild.</param>
        /// <returns>The Discord.NET Embed object.</returns>
        public Embed BuildUsersEmbed(List<DiscordUser> discordUsers, IGuild guild)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            StringBuilder builder = new StringBuilder();

            foreach (var user in discordUsers)
            {
                if (user.AnilistId == 0)
                {
                    continue;
                }

                builder.Append($"**Discord**: {user.Name} - **Anilist**: [{user.AnilistName}](https://anilist.co/user/{user.AnilistId}) \n");
            }

            embedBuilder.WithDescription(CutStringWithEllipsis(builder.ToString()));

            embedBuilder.WithColor(Color.DarkRed)
                .WithThumbnailUrl(guild.IconUrl)
                .WithTitle($"Anilist users in {guild.Name}");

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

        /// <summary>
        /// Convert a string to its equivalent Discord colour.
        /// </summary>
        /// <param name="colour">The colour as a string.</param>
        /// <returns>A Tuple object containing RGB values. Defaults to black.</returns>
        private Tuple<int, int, int> ConvertStringToDiscordColour(string colour)
        {
            // All these RG
            return colour switch
            {
                "blue" => Tuple.Create(61, 180, 242),
                "purple" => Tuple.Create(192, 99, 255),
                "green" => Tuple.Create(76, 202, 81),
                "orange" => Tuple.Create(239, 136, 26),
                "red" => Tuple.Create(225, 51, 51),
                "pink" => Tuple.Create(252, 157, 214),
                "gray" => Tuple.Create(103, 123, 148),
                _ => Tuple.Create(0, 0, 0),
            };
        }
    }
}
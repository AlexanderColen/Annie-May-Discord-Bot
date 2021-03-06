﻿using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Properties;
using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("help")]
    public class HelpModule : ModuleBase<CustomCommandContext>
    {
        /// <summary>
        /// Shows and overview of the bot commands.
        /// </summary>
        /// <returns>An Embed reply containing all the commands.</returns>
        [Command]
        public Task HelpAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            List<(string Command, string Description)> commandList = new List<(string Command, string Description)>();
            commandList.Add(("about", "Find out what kind of bot I am."));
            commandList.Add(("search `CRITERIA`", "Search AniList's database for media based on the given criteria. Returns a list of entries."));
            commandList.Add(("find `CRITERIA`", "Finds one piece of media from AniList's database."));
            commandList.Add(("anime `CRITERIA`", "Finds one piece of anime from AniList's database."));
            commandList.Add(("manga `CRITERIA`", "Finds one piece of manga from AniList's database."));
            commandList.Add(("character `CRITERIA`", "Finds one character from AniList's database."));
            commandList.Add(("staff `CRITERIA`", "Finds one staff from AniList's database."));
            commandList.Add(("studio `CRITERIA`", "Finds one studio from AniList's database."));
            commandList.Add(("user `ANILIST_USERNAME`", "Shows a User's Anilist statistics."));
            commandList.Add(("scores `ANILIST_USERNAME`", "Shows a User's Anilist scores."));
            commandList.Add(("setup anilist `ANILIST_USERNAME`", "Adds a User's Anilist to the database for future usage."));
            commandList.Add(("settings", "Change the prefix and user scores settings for a specific server."));
            commandList.Add(("random `ACTION`", "Generate some random recommendations, dice rolls, coinflips, and more."));
            commandList.Add(("affinity", "Calculate the affinity between the user and all other users in this server."));

            foreach (var (Command, Description) in commandList.OrderBy(x => x.Command))
            {
                builder.AddField($"{Resources.PREFIX}{Command}", Description);
            }

            builder.WithTitle("Commands Overview")
                .WithDescription($"For more descriptive help, type {Resources.PREFIX}help `COMMAND`")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the search command.
        /// </summary>
        /// <returns>An Embed reply regarding the Search command.</returns>
        [Command("search")]
        [Alias("search anime", "search manga", "search character", "search characters", "search staff", "search studio", "search studios")]
        public Task HelpSearchAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}search")
                .AddField($"{Resources.PREFIX}search anime `CRITERIA`", "Specify the search to anime only.")
                .AddField($"{Resources.PREFIX}search manga `CRITERIA`", "Specify the search to manga only.")
                .AddField($"{Resources.PREFIX}search characters `CRITERIA`", "Specify the search to characters only.")
                .AddField($"{Resources.PREFIX}search staff `CRITERIA`", "Specify the search to staff only.")
                .AddField($"{Resources.PREFIX}search studios `CRITERIA`", "Specify the search to studios only.")
                .WithDescription($"Searches through AniList's database to find items based on the given criteria.\n\n" +
                $"_Regex_ `search (anime|manga|char(acters?)?|staff|studios?)? (.+)+`\n\n" +
                $"Example usage: `{Resources.PREFIX}search sword art online`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the find command.
        /// </summary>
        /// <returns>An Embed reply regarding the Find command.</returns>
        [Command("find")]
        [Alias("find anime", "find manga", "fetch", "fetch anime", "fetch manga", "get", "get anime", "get manga", "media", "media anime", "media manga")]
        public Task HelpFindAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}find")
                .AddField($"{Resources.PREFIX}find `CRITERIA`", "Specify the find to anime only based on search criteria.")
                .AddField($"{Resources.PREFIX}find anime `CRITERIA`", "Specify the find to anime only based on search criteria.")
                .AddField($"{Resources.PREFIX}find anime `ID`", "Specify the find to anime only based on the Anilist ID.")
                .AddField($"{Resources.PREFIX}find manga `CRITERIA`", "Specify the find to manga only based on search criteria.")
                .AddField($"{Resources.PREFIX}find manga `ID`", "Specify the find to manga only based on the Anilist ID.")
                .WithDescription($"Finds a single piece of media based on the given criteria or ID.\n\nExample usage: `{Resources.PREFIX}find fullmetal alchemist brotherhood`\n\n" +
                $"_Regex_ `find (anime|manga)? (.+)+`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._\n\n" +
                $"Aliases: [get, fetch, media]")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the anime command.
        /// </summary>
        /// <returns>An Embed reply regarding the Anime command.</returns>
        [Command("anime")]
        public Task HelpAnimeAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}anime")
                .AddField($"{Resources.PREFIX}anime `CRITERIA`", "Find a single anime based on criteria.")
                .AddField($"{Resources.PREFIX}anime `ID`", "Find a single anime based on ID.")
                .WithDescription($"Finds a single piece of anime based on the given criteria or ID.\n\n" +
                $"_Regex_ `anime (.+)+`\n\n" +
                $"Example usage: `{Resources.PREFIX}anime kimi no na wa`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the manga command.
        /// </summary>
        /// <returns>An Embed reply regarding the Manga command.</returns>
        [Command("manga")]
        public Task HelpMangaAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}manga")
                .AddField($"{Resources.PREFIX}manga `CRITERIA`", "Find a single manga based on criteria.")
                .AddField($"{Resources.PREFIX}manga `ID`", "Find a single manga based on ID.")
                .WithDescription($"Finds a single piece of manga based on the given criteria or ID.\n\n" +
                $"_Regex_ `manga (.+)+`\n\n" +
                $"Example usage: `{Resources.PREFIX}manga sword art online`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the character command.
        /// </summary>
        /// <returns>An Embed reply regarding the Character command.</returns>
        [Command("character")]
        [Alias("char")]
        public Task HelpCharacterAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}character")
                .AddField($"{Resources.PREFIX}character `CRITERIA`", "Find a single character based on criteria.")
                .AddField($"{Resources.PREFIX}character `ID`", "Find a single character based on ID.")
                .WithDescription($"Finds a single character based on the given criteria or ID.\n\n" +
                $"_Regex_ `(char(acters?)? ){1}(.+)+`\n\n" +
                $"Example usage: `{Resources.PREFIX}character tokisaki kurumi`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the staff command.
        /// </summary>
        /// <returns>An Embed reply regarding the Staff command.</returns>
        [Command("staff")]
        public Task HelpStaffAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}staff")
                .AddField($"{Resources.PREFIX}staff `CRITERIA`", "Find a single staff based on criteria.")
                .AddField($"{Resources.PREFIX}staff `ID`", "Find a single staff based on ID.")
                .WithDescription($"Finds a single staff based on the given criteria or ID.\n\n" +
                $"_Regex_ `staff (.+)+`\n\n" +
                $"Example usage: `{Resources.PREFIX}staff tomoyo kurosawa`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the studio command.
        /// </summary>
        /// <returns>An Embed reply regarding the Studio command.</returns>
        [Command("studio")]
        public Task HelpStudioAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}studio")
                .AddField($"{Resources.PREFIX}studio `CRITERIA`", "Find a single studio based on criteria.")
                .AddField($"{Resources.PREFIX}studio `ID`", "Find a single studio based on ID.")
                .WithDescription($"Finds a single studio based on the given criteria or ID.\n\n" +
                $"_Regex_ `studio (.+)+`\n\n" +
                $"Example usage: `{Resources.PREFIX}studio kyoto animation`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the user command.
        /// </summary>
        /// <returns>An Embed reply regarding the User command.</returns>
        [Command("user")]
        [Alias("list", "userlist")]
        public Task HelpUserAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}user")
                .AddField($"{Resources.PREFIX}user `USERNAME`", "Specify the username of the user.")
                .AddField($"{Resources.PREFIX}user `ID`", "Specify the id of the user.")
                .WithDescription($"Finds the user with the given username or ID and displays their anime & manga list statistics.\n\n" +
                $"_Regex_ `(user)?(list)? (\\w+|<@\\?\\d+>)`\n\n" +
                $"Example usage: `{Resources.PREFIX}user SmellyAlex`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the scores command.
        /// </summary>
        /// <returns>An Embed reply regarding the Scores command.</returns>
        [Command("scores")]
        [Alias("scoredistribution", "userscores")]
        public Task HelpScoresAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"{Resources.PREFIX}scores")
                .AddField($"{Resources.PREFIX}scores `USERNAME`", "Specify the Anilist username of the user.")
                .AddField($"{Resources.PREFIX}scores `ID`", "Specify the Discord or Anilist id of the user.")
                .WithDescription($"Finds the user with the given username or ID and displays their Anilist scores breakdown.\n\n" +
                $"_Regex_ `((user)?scores|scoredistribution) ?((@<\\?)?\\w+>?)+ ?(anime|manga)* ?((<|>|=|<=|>=){1}\\d+ ?)*`\n\n" +
                $"Example usage: `{Resources.PREFIX}scores SmellyAlex`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the setup command.
        /// </summary>
        /// <returns>An Embed reply regarding the Setup command.</returns>
        [Command("setup")]
        [Alias("setup anilist", "setup edit", "setup update")]
        public Task HelpSetupAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}setup")
                .AddField($"{Resources.PREFIX}setup anilist `USERNAME`", "Looks for the Anilist user based on username and adds it to the database.")
                .AddField($"{Resources.PREFIX}setup anilist `ID`", "Looks for the Anilist user based on ID and adds it to the database.")
                .AddField($"{Resources.PREFIX}setup update `USERNAME`", "Edits the User's Anilist information in the database based on new Anilist username.")
                .AddField($"{Resources.PREFIX}setup update `ID`", "Edits the User's Anilist information in the database based on new Anilist ID.")
                .WithDescription($"Adds a User's Anilist to the database for future usage.\n\n" +
                $"_Regex_ `setup( (anilist|edit|update) \\w+)?`\n\n" +
                $"Example usage: `{Resources.PREFIX}setup anilist 210768`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the settings command.
        /// </summary>
        /// <returns>An Embed reply regarding the Settings command.</returns>
        [Command("settings")]
        [Alias("prefix", "settings prefix", "settings userscores", "settings scoring", "settings scores", "guild prefix", "guild userscores", "guild scoring", "guild scores", "server prefix", "server userscores", "server scoring", "server scores")]
        public Task HelpSettingsAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}settings")
                .AddField($"{Resources.PREFIX}settings prefix `PREFIX`", "Change the prefix for a server.")
                .AddField($"{Resources.PREFIX}settings userscores `true/false`", "Enable/disable the displaying of User's scores on media search for a server.")
                .WithDescription($"Change settings for the server that the message was sent in.\nOnly server administrators are allowed to do these actions.\n\n" +
                $"_Regex_ `settings( ((prefix .+)|((userscores|scoring|scores) (true|false))))?`\n\n" +
                $"Example usage: `{Resources.PREFIX}settings prefix $`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the random command.
        /// </summary>
        /// <returns>An Embed reply regarding the Random command.</returns>
        [Command("random")]
        [Alias("roll", "die", "dice", "coinflip", "coin", "flip", "random roll", "random die", "random dice", "random coinflip", "random coin", "random flip", "ptw", "ptr", "plantowatch", "plantoread", "planned", "planning", "random ptw", "random ptr", "random plantowatch", "random plantoread", "random planned", "random planning")]
        public Task HelpRandomAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}random")
                .AddField($"{Resources.PREFIX}random roll", "Roll a random positive integer.")
                .AddField($"{Resources.PREFIX}random roll `INTEGER`", "Roll a random integer up to the given maximum.")
                .AddField($"{Resources.PREFIX}random roll `INTEGER` `INTEGER`", "Roll a random integer between a given minimum and maximum.")
                .AddField($"{Resources.PREFIX}random coinflip", "Flips a coin.")
                .AddField($"{Resources.PREFIX}random planning", "Picks a random entry from the attached Anilist user's Planning list.")
                .WithDescription($"Generate some random dice rolls, coinflips or recommendation.\n\n" +
                $"_Regex_ `((random) ?)?(coin)?(flip)?|(roll|dic?e)( -?\\d+( -?\\d+)?)?|p((t(w|r))|(lan(n(ed|ing)|to(watch|read))))`\n\n" +
                $"Example usage: `{Resources.PREFIX}roll 1 10 $`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the affinity command.
        /// </summary>
        /// <returns>An Embed reply regarding the Affinity command.</returns>
        [Command("affinity")]
        [Alias("affinities", "similarity", "similarities")]
        public Task HelpAffinityAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}affinity")
                .AddField($"{Resources.PREFIX}affinity `USERNAME`", "Specify the username of the user.")
                .AddField($"{Resources.PREFIX}affinity `ID`", "Specify the ID of the user.")
                .AddField($"{Resources.PREFIX}affinity `USERNAME` `USERNAME`", "Specify the username of both users.")
                .AddField($"{Resources.PREFIX}affinity `ID` `ID`", "Specify the ID of both users.")
                .WithDescription($"Finds the user(s) with the given username(s) or ID(s) and displays their affinity.\n\n" +
                $"_Regex_ `((affinit|similarit)(y|(ies)))( (\\w+|<@\\!\\d+>)){0,2}`\n\n" +
                $"Example usage: `{Resources.PREFIX}affinity SmellyAlex`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }
    }
}
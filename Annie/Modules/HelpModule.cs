using Discord;
using Discord.Interactions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class HelpModule : AbstractInteractionModule
    {
        public enum HelpType
        {
            Affinity,
            Anime,
            Character,
            Find,
            Manga,
            Random,
            Search,
            Scores,
            Setup,
            Staff,
            Studio,
            ToggleUserScores,
            User
        }

        /// <summary>
        /// Shows and overview of the bot commands.
        /// </summary>
        /// <returns>An Embed reply containing all the commands.</returns>
        [SlashCommand("help", "Show help with using this bot")]
        public Task HelpAsync(HelpType type)
        {
            Embed embed;

            switch (type)
            {
                case HelpType.Affinity:
                    embed = HelpAffinity();
                    break;

                case HelpType.Anime:
                    embed = HelpAnime();
                    break;

                case HelpType.Character:
                    embed = HelpCharacter();
                    break;

                case HelpType.Find:
                    embed = HelpFind();
                    break;

                case HelpType.Manga:
                    embed = HelpManga();
                    break;

                case HelpType.Random:
                    embed = HelpRandom();
                    break;

                case HelpType.Scores:
                    embed = HelpScores();
                    break;

                case HelpType.Search:
                    embed = HelpSearch();
                    break;

                case HelpType.Setup:
                    embed = HelpSetup();
                    break;

                case HelpType.Staff:
                    embed = HelpStaff();
                    break;

                case HelpType.Studio:
                    embed = HelpStudio();
                    break;

                case HelpType.ToggleUserScores:
                    embed = HelpSettings();
                    break;

                case HelpType.User:
                    embed = HelpUser();
                    break;

                default:
                    embed = Help();
                    break;
            }

            return RespondAsync(isTTS: false, embed: embed, ephemeral: true);
        }

        /// <summary>
        /// Shows help for the bot.
        /// </summary>
        /// <returns>An Embed reply regarding the bot commands.</returns>
        private Embed Help()
        {
            EmbedBuilder builder = new EmbedBuilder();

            List<(string Command, string Description)> commandList = new List<(string Command, string Description)>
            {
                ("about", "Find out what kind of bot I am."),
                ("search `CRITERIA`", "Search AniList's database for media based on the given criteria. Returns a list of entries."),
                ("find `CRITERIA`", "Finds one piece of media from AniList's database."),
                ("anime `CRITERIA`", "Finds one piece of anime from AniList's database."),
                ("manga `CRITERIA`", "Finds one piece of manga from AniList's database."),
                ("character `CRITERIA`", "Finds one character from AniList's database."),
                ("staff `CRITERIA`", "Finds one staff from AniList's database."),
                ("studio `CRITERIA`", "Finds one studio from AniList's database."),
                ("user `ANILIST_USERNAME`", "Shows a User's Anilist statistics."),
                ("scores `ANILIST_USERNAME`", "Shows a User's Anilist scores."),
                ("setup anilist `ANILIST_USERNAME`", "Adds a User's Anilist to the database for future usage."),
                ("settings", "Change the prefix and user scores settings for a specific server."),
                ("random `ACTION`", "Generate some random recommendations, dice rolls, coinflips, and more."),
                ("affinity", "Calculate the affinity between the user and all other users in this server.")
            };

            foreach (var (Command, Description) in commandList.OrderBy(x => x.Command))
            {
                builder.AddField($"/{Command}", Description);
            }

            builder.WithTitle("Commands Overview")
                .WithDescription($"For more descriptive help, type /help `COMMAND`")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the search command.
        /// </summary>
        /// <returns>An Embed reply regarding the Search command.</returns>
        private Embed HelpSearch()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("/search")
                .AddField("/search anime `CRITERIA`", "Specify the search to anime only.")
                .AddField("/search manga `CRITERIA`", "Specify the search to manga only.")
                .AddField("/search characters `CRITERIA`", "Specify the search to characters only.")
                .AddField("/search staff `CRITERIA`", "Specify the search to staff only.")
                .AddField("/search studios `CRITERIA`", "Specify the search to studios only.")
                .WithDescription("Searches through AniList's database to find items based on the given criteria.\n\n" +
                "_Regex_ `search (anime|manga|characters|staff|studios) (.+)+`\n\n" +
                "Example usage: `/search anime sword art online`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the find command.
        /// </summary>
        /// <returns>An Embed reply regarding the Find command.</returns>
        private Embed HelpFind()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"/find")
                .AddField($"/find `CRITERIA`", "Specify the find to anime only based on search criteria.")
                .AddField($"/find anime `CRITERIA`", "Specify the find to anime only based on search criteria.")
                .AddField($"/find anime `ID`", "Specify the find to anime only based on the Anilist ID.")
                .AddField($"/find manga `CRITERIA`", "Specify the find to manga only based on search criteria.")
                .AddField($"/find manga `ID`", "Specify the find to manga only based on the Anilist ID.")
                .WithDescription($"Finds a single piece of media based on the given criteria or ID.\n\nExample usage: `/find fullmetal alchemist brotherhood`\n\n" +
                $"_Regex_ `find (anime|manga)? (.+)+`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._\n\n" +
                $"Aliases: [get, fetch, media]")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the anime command.
        /// </summary>
        /// <returns>An Embed reply regarding the Anime command.</returns>
        private Embed HelpAnime()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"/anime")
                .AddField($"/anime `CRITERIA`", "Find a single anime based on criteria.")
                .AddField($"/anime `ID`", "Find a single anime based on ID.")
                .WithDescription($"Finds a single piece of anime based on the given criteria or ID.\n\n" +
                $"_Regex_ `anime (.+)+`\n\n" +
                $"Example usage: `/anime kimi no na wa`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the manga command.
        /// </summary>
        /// <returns>An Embed reply regarding the Manga command.</returns>
        private Embed HelpManga()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"/manga")
                .AddField($"/manga `CRITERIA`", "Find a single manga based on criteria.")
                .AddField($"/manga `ID`", "Find a single manga based on ID.")
                .WithDescription($"Finds a single piece of manga based on the given criteria or ID.\n\n" +
                $"_Regex_ `manga (.+)+`\n\n" +
                $"Example usage: `/manga sword art online`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the character command.
        /// </summary>
        /// <returns>An Embed reply regarding the Character command.</returns>
        private Embed HelpCharacter()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"/character")
                .AddField($"/character `CRITERIA`", "Find a single character based on criteria.")
                .AddField($"/character `ID`", "Find a single character based on ID.")
                .WithDescription($"Finds a single character based on the given criteria or ID.\n\n" +
                $"_Regex_ `(char(acters?)? ){1}(.+)+`\n\n" +
                $"Example usage: `/character tokisaki kurumi`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the staff command.
        /// </summary>
        /// <returns>An Embed reply regarding the Staff command.</returns>
        private Embed HelpStaff()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"/staff")
                .AddField($"/staff `CRITERIA`", "Find a single staff based on criteria.")
                .AddField($"/staff `ID`", "Find a single staff based on ID.")
                .WithDescription($"Finds a single staff based on the given criteria or ID.\n\n" +
                $"_Regex_ `staff (.+)+`\n\n" +
                $"Example usage: `/staff tomoyo kurosawa`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the studio command.
        /// </summary>
        /// <returns>An Embed reply regarding the Studio command.</returns>
        private Embed HelpStudio()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"/studio")
                .AddField($"/studio `CRITERIA`", "Find a single studio based on criteria.")
                .AddField($"/studio `ID`", "Find a single studio based on ID.")
                .WithDescription($"Finds a single studio based on the given criteria or ID.\n\n" +
                $"_Regex_ `studio (.+)+`\n\n" +
                $"Example usage: `/studio kyoto animation`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the user command.
        /// </summary>
        /// <returns>An Embed reply regarding the User command.</returns>
        private Embed HelpUser()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"/user")
                .AddField($"/user `USERNAME`", "Specify the username of the user.")
                .AddField($"/user `ID`", "Specify the id of the user.")
                .WithDescription($"Finds the user with the given username or ID and displays their anime & manga list statistics.\n\n" +
                $"_Regex_ `(user)?(list)? (\\w+|<@\\?\\d+>)`\n\n" +
                $"Example usage: `/user SmellyAlex`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the scores command.
        /// </summary>
        /// <returns>An Embed reply regarding the Scores command.</returns>
        private Embed HelpScores()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"/scores")
                .AddField($"/scores `USERNAME`", "Specify the Anilist username of the user.")
                .AddField($"/scores `ID`", "Specify the Discord or Anilist id of the user.")
                .WithDescription($"Finds the user with the given username or ID and displays their Anilist scores breakdown.\n\n" +
                $"_Regex_ `((user)?scores|scoredistribution) ?((@<\\?)?\\w+>?)+ ?(anime|manga)* ?((<|>|=|<=|>=){1}\\d+ ?)*`\n\n" +
                $"Example usage: `/scores SmellyAlex`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the setup command.
        /// </summary>
        /// <returns>An Embed reply regarding the Setup command.</returns>
        private Embed HelpSetup()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"/setup")
                .AddField($"/setup anilist `USERNAME`", "Looks for the Anilist user based on username and adds it to the database.")
                .AddField($"/setup anilist `ID`", "Looks for the Anilist user based on ID and adds it to the database.")
                .AddField($"/setup update `USERNAME`", "Edits the User's Anilist information in the database based on new Anilist username.")
                .AddField($"/setup update `ID`", "Edits the User's Anilist information in the database based on new Anilist ID.")
                .WithDescription($"Adds a User's Anilist to the database for future usage.\n\n" +
                $"_Regex_ `setup( (anilist|edit|update) \\w+)?`\n\n" +
                $"Example usage: `/setup anilist 210768`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the settings command.
        /// </summary>
        /// <returns>An Embed reply regarding the Settings command.</returns>
        private Embed HelpSettings()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"/settings")
                .AddField($"/settings prefix `PREFIX`", "Change the prefix for a server.")
                .AddField($"/settings userscores `true/false`", "Enable/disable the displaying of User's scores on media search for a server.")
                .WithDescription($"Change settings for the server that the message was sent in.\nOnly server administrators are allowed to do these actions.\n\n" +
                $"_Regex_ `settings( ((prefix .+)|((userscores|scoring|scores) (true|false))))?`\n\n" +
                $"Example usage: `/settings prefix $`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the random command.
        /// </summary>
        /// <returns>An Embed reply regarding the Random command.</returns>
        private Embed HelpRandom()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"/random")
                .AddField($"/random roll", "Roll a random positive integer.")
                .AddField($"/random roll `INTEGER`", "Roll a random integer up to the given maximum.")
                .AddField($"/random roll `INTEGER` `INTEGER`", "Roll a random integer between a given minimum and maximum.")
                .AddField($"/random coinflip", "Flips a coin.")
                .AddField($"/random planning", "Picks a random entry from the attached Anilist user's Planning list.")
                .WithDescription($"Generate some random dice rolls, coinflips or recommendation.\n\n" +
                $"_Regex_ `((random) ?)?(coin)?(flip)?|(roll|dic?e)( -?\\d+( -?\\d+)?)?|p((t(w|r))|(lan(n(ed|ing)|to(watch|read))))`\n\n" +
                $"Example usage: `/roll 1 10 $`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }

        /// <summary>
        /// Shows help for the affinity command.
        /// </summary>
        /// <returns>An Embed reply regarding the Affinity command.</returns>
        private Embed HelpAffinity()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"/affinity")
                .AddField($"/affinity `USERNAME`", "Specify the username of the user.")
                .AddField($"/affinity `ID`", "Specify the ID of the user.")
                .AddField($"/affinity `USERNAME` `USERNAME`", "Specify the username of both users.")
                .AddField($"/affinity `ID` `ID`", "Specify the ID of both users.")
                .WithDescription($"Finds the user(s) with the given username(s) or ID(s) and displays their affinity.\n\n" +
                $"_Regex_ `((affinit|similarit)(y|(ies)))( (\\w+|<@\\!\\d+>)){0,2}`\n\n" +
                $"Example usage: `/affinity SmellyAlex`\n\n" +
                $"_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return builder.Build();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnnieMayDiscordBot.Properties;
using Discord;
using Discord.Commands;

namespace AnnieMayDiscordBot.Modules
{
    [Group("help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Shows and overview of the bot commands.
        /// </summary>
        /// <returns>An Embed reply containing all the commands.</returns>
        [Command]
        public Task HelpAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Commands Overview")
                .AddField($"{Resources.PREFIX}search `CRITERIA`", "Search AniList's database for media based on the given criteria. Returns a list of entries.")
                .AddField($"{Resources.PREFIX}find `CRITERIA`", "Finds one piece of media from AniList's database.")
                .AddField($"{Resources.PREFIX}anime `CRITERIA`", "Finds one piece of anime from AniList's database.")
                .AddField($"{Resources.PREFIX}manga `CRITERIA`", "Finds one piece of manga from AniList's database.")
                .AddField($"{Resources.PREFIX}user `ANILIST_USERNAME`", "Shows a User's Anilist statistics.")
                .AddField($"{Resources.PREFIX}setup anilist `ANILIST_USERNAME`", "Adds a User's Anilist to the database for future usage.")
                .WithDescription($"For more descriptive help, type {Resources.PREFIX}help `COMMAND`")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Shows help for the search command.
        /// </summary>
        /// <returns>An Embed reply regarding the Search command.</returns>
        [Command("search")]
        public Task HelpSearchAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}search")
                .AddField($"{Resources.PREFIX}search anime `CRITERIA`", "Specify the search to anime only.")
                .AddField($"{Resources.PREFIX}search manga `CRITERIA`", "Specify the search to manga only.")
                .WithDescription($"Searches through AniList's database to find media based on the given criteria.\n\nExample usage: `{Resources.PREFIX}search sword art online`\n\n_{builder.Fields.Count} overloads exist for this command._")
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
                .WithDescription($"Finds a single piece of media based on the given criteria or ID.\n\nExample usage: `{Resources.PREFIX}find fullmetal alchemist brotherhood`\n\n_{builder.Fields.Count} overloads exist for this command._\n\nAliases: [get, fetch, media]")
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
                .WithDescription($"Finds a single piece of anime based on the given criteria or ID.\n\nExample usage: `{Resources.PREFIX}anime kimi no na wa`\n\n_{builder.Fields.Count} overloads exist for this command._")
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
                .WithDescription($"Finds a single piece of manga based on the given criteria or ID.\n\nExample usage: `{Resources.PREFIX}manga sword art online`\n\n_{builder.Fields.Count} overloads exist for this command._")
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
                .WithDescription($"Finds the user with the given username or ID and displays their anime & manga list statistics.\n\nExample usage: `{Resources.PREFIX}user SmellyAlex`\n\n_{builder.Fields.Count} overloads exist for this command._")
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
                .WithDescription($"Adds a User's Anilist to the database for future usage.\n\nExample usage: `{Resources.PREFIX}setup anilist 210768`\n\n_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }
    }
}

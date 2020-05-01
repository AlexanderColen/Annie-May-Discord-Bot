using System.Threading.Tasks;
using AnnieMayDiscordBot.Properties;
using Discord;
using Discord.Commands;

namespace AnnieMayDiscordBot.Modules
{
    [Group("help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Summary("Shows and overview of the bot commands.")]
        public Task HelpAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Commands Overview")
                .AddField($"{Resources.PREFIX}search `CRITERIA`", "Search AniList's database for media based on the given criteria. Returns a list of entries.")
                .AddField($"{Resources.PREFIX}find `CRITERIA`", "Finds one piece of media from AniList's database.")
                .WithDescription($"For more descriptive help, type {Resources.PREFIX}help `COMMAND`")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        [Command("search")]
        [Summary("Shows help for the search command.")]
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

        [Command("find")]
        [Summary("Shows help for the find command.")]
        public Task HelpFindAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Resources.PREFIX}find")
                .AddField($"{Resources.PREFIX}find anime `CRITERIA`", "Specify the find to anime only.")
                .AddField($"{Resources.PREFIX}find manga `CRITERIA`", "Specify the find to manga only.")
                .WithDescription($"Finds a single piece of media based on the given criteria.\n\nExample usage: `{Resources.PREFIX}find sword art online`\n\n_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }
    }
}

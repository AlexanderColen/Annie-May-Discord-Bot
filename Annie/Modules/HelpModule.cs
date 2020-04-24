using Discord;
using Discord.Commands;
using System.Threading.Tasks;

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
                .AddField($"{Properties.Resources.PREFIX}search `CRITERIA`", "Search AniList's database for media based on the given criteria. Returns a list of entries.", false)
                .AddField($"{Properties.Resources.PREFIX}find `CRITERIA`", "Finds one piece of media from AniList's database.", false)
                .WithDescription($"For more descriptive help, type {Properties.Resources.PREFIX}help `COMMAND`")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        [Command("search")]
        [Summary("Shows help for the search command.")]
        public Task HelpSearchAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Properties.Resources.PREFIX}search")
                .AddField($"{Properties.Resources.PREFIX}search anime `CRITERIA`", "Specify the search to anime only.", false)
                .AddField($"{Properties.Resources.PREFIX}search manga `CRITERIA`", "Specify the search to manga only.", false)
                .WithDescription($"Searches through AniList's database to find media based on the given criteria.\n\nExample usage: `{Properties.Resources.PREFIX}search sword art online`\n\n_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }

        [Command("find")]
        [Summary("Shows help for the find command.")]
        public Task HelpFindAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Properties.Resources.PREFIX}find")
                .AddField($"{Properties.Resources.PREFIX}find anime `CRITERIA`", "Specify the find to anime only.", false)
                .AddField($"{Properties.Resources.PREFIX}find manga `CRITERIA`", "Specify the find to manga only.", false)
                .WithDescription($"Finds a single piece of media based on the given criteria.\n\nExample usage: `{Properties.Resources.PREFIX}find sword art online`\n\n_{builder.Fields.Count} overloads exist for this command._")
                .WithColor(Color.DarkRed);

            return ReplyAsync("", false, builder.Build());
        }
    }
}

using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("guild")]
    [Alias("settings", "server")]
    public class SettingsModule : AbstractModule
    {
        /// <summary>
        /// Default catch for Settings telling user what to do next.
        /// </summary>
        [Command]
        [Summary("Start the guild settings process.")]
        public async Task SettingsAsync()
        {
            await Context.Channel.SendMessageAsync("Guild settings are set for entire server, and therefore only the server administrators can change them.\n\n" +
                "**Prefix** - Change the prefix for this server. Usage: `guild prefix <prefix>`.\n" +
                "**User Scores** - Enable/Disable showing User's scores when looking for media. Usage: `guild userscores <true/false>`");
        }

        /// <summary>
        /// Set a custom prefix for a server.
        /// </summary>
        [Command("prefix")]
        [Summary("Set a custom prefix for a server.")]
        public async Task SettingsPrefixAsync([Remainder] string prefix)
        {
            // Check if the user is an administrator.
            if (!await IsUserAdministrator())
            {
                await Context.Channel.SendMessageAsync("Only server administrators are allowed to do this.");
                return;
            }

            var guildSettings = Context.Settings;

            // Change prefix if it exists.
            if (guildSettings != null)
            {
                guildSettings.Prefix = prefix;
            }
            // Otherwise create a new one.
            else
            {
                guildSettings = new GuildSettings()
                {
                    GuildId = Context.Guild.Id,
                    Prefix = prefix
                };
            }

            // Update in the database.
            if (await DatabaseUtility.GetInstance().UpsertGuildSettings(guildSettings))
            {
                await Context.Message.AddReactionAsync(new Emoji("\u2611"));
            }
        }

        /// <summary>
        /// Enable/Disable showing User's scores when looking for media for a server.
        /// </summary>
        [Command("userscores")]
        [Alias("scores", "scoring")]
        [Summary("Enable/Disable showing User's scores when looking for media for a server.")]
        public async Task SettingsUserScoringAsync([Remainder] string userScoresBool)
        {
            // Check if the user is an administrator.
            if (!await IsUserAdministrator()) {
                await Context.Channel.SendMessageAsync("Only server administrators are allowed to do this.");
                return;
            }

            // Try to parse the given parameter to a boolean.
            if (!bool.TryParse(userScoresBool, out bool showUserScores))
            {
                await Context.Channel.SendMessageAsync("The given parameter is neither `true` nor `false` and could not be parsed.");
                return;
            }

            var guildSettings = Context.Settings;

            // Change user scores setting if it exists.
            if (guildSettings != null)
            {
                guildSettings.ShowUserScores = showUserScores;
            }
            // Otherwise create a new one.
            else
            {
                guildSettings = new GuildSettings()
                {
                    GuildId = Context.Guild.Id,
                    // Use the default prefix for now.
                    Prefix = Properties.Resources.PREFIX,
                    ShowUserScores = showUserScores
                };
            }

            // Update in the database.
            if (await DatabaseUtility.GetInstance().UpsertGuildSettings(guildSettings))
            {
                await Context.Message.AddReactionAsync(new Emoji("\u2611"));
            }
        }

        /// <summary>
        /// Check whether the context User is an administrator in the Guild that the message was sent.
        /// </summary>
        /// <returns>True if they are an administrator, false otherwise.</returns>
        private async Task<bool> IsUserAdministrator()
        {
            var user = await Context.Guild.GetUserAsync(Context.User.Id, CacheMode.AllowDownload);

            return user.GuildPermissions.Administrator;
        }
    }
}
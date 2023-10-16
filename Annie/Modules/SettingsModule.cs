using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Interactions;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class SettingsModule : AbstractInteractionModule
    {
        public enum OnOff
        {
            On,
            Off
        }

        /// <summary>
        /// Enable/Disable showing User's scores when looking for media for a server.
        /// </summary>
        [SlashCommand("toggle-user-scores", "Enable/Disable showing User's scores when looking for media in this guild.")]
        public async Task SettingsUserScoringAsync([Summary(name: "on-off")] OnOff showUserScores)
        {
            // Check if the user is an administrator.
            if (!IsUserAdministrator()) {
                await FollowupAsync(text: "Only server administrators are allowed to do this.", ephemeral: true);
                return;
            }

            // Get the settings that should be used for this Guild.
            CacheUtility.GetInstance().CachedGuildSettings.TryGetValue(Context.Guild.Id, out GuildSettings guildSettings);

            // Change user scores setting if it exists.
            if (guildSettings != null)
            {
                guildSettings.ShowUserScores = showUserScores == OnOff.On;
            }
            // Create the settings if it doesn't exist.
            else
            {
                guildSettings = new GuildSettings
                {
                    GuildId = Context.Guild.Id,
                    ShowUserScores = showUserScores == OnOff.On
                };

                // Make sure to add guild settings to the dictionary to prevent future unnecessary database querying.
                CacheUtility.GetInstance().CachedGuildSettings.Add(Context.Guild.Id, guildSettings);
            }

            // Update in the database.
            if (await DatabaseUtility.GetInstance().UpsertGuildSettings(guildSettings))
            {
                await FollowupAsync(text: $"Updated! {new Emoji("\u2611")} This guild will {(showUserScores == OnOff.On ? "now" : "no longer")} see user scores when fetching media", ephemeral: true);
            }
        }

        /// <summary>
        /// Check whether the context User is an administrator in the Guild that the message was sent.
        /// </summary>
        /// <returns>True if they are an administrator, false otherwise.</returns>
        private bool IsUserAdministrator()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            return user.GuildPermissions.Administrator;
        }
    }
}
using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Interactions;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("setup", "Setup personal AniList use of the bot.")]
    public class SetupModule : AbstractInteractionModule
    {
        /// <summary>
        /// Default catch for Setup telling user what to do next.
        /// </summary>
        [SlashCommand("", "Start the setup process.")]
        public async Task SetupAsync()
        {
            await RespondAsync(text: "Annie May at your service, your friendly e-neighbourhood Anilist bot!\n\n" +
                "Register or update your anilist using `setup anilist <username/id>`.", ephemeral: true);
        }

        /// <summary>
        /// Setup to register a DiscordUser with their Anilist username.
        /// </summary>
        /// <param name="args">String indicating their Anilist username or ID.</param>
        [SlashCommand("anilist", "Add or update your AniList guild participation to the database using username or ID.")]
        public async Task SetupAnilistAsync(
            [Summary(name: "anilist-username-or-id", description:  "AniList username or ID to register yourself with")] string args)
        {
            if (int.TryParse(args, out int anilistId))
            {
                if (await UpsertAnilistUser(null, anilistId))
                {
                    await RespondAsync(text: $"Success! {new Emoji("\u2611")}", ephemeral: true);
                }
            } else
            {
                if (await UpsertAnilistUser(args, 0))
                {
                    await RespondAsync(text: $"Success! {new Emoji("\u2611")}", ephemeral: true);
                }
            }

            
        }

        /// <summary>
        /// Insert or update a DiscordUser's Anilist name and/or ID in the database.
        /// </summary>
        /// <param name="anilistName">The name of their Anilist account.</param>
        /// <param name="anilistId">The ID of their Anilist account.</param>
        /// <returns>True if the new DiscordUser is inserted or updated in the database, otherwise false.</returns>
        private async Task<bool> UpsertAnilistUser(string anilistName, int anilistId)
        {
            User anilistUser = null;
            // Find the User with the Anilist name if is it not null or empty.
            if (!string.IsNullOrEmpty(anilistName))
            {
                UserResponse userResponse = await _aniListFetcher.FindUserAsync(anilistName);
                anilistUser = userResponse.User;
            }
            // Find the User with the Anilist ID if is it not null or empty.
            else if (anilistId != 0)
            {
                UserResponse userResponse = await _aniListFetcher.FindUserAsync(anilistId);
                anilistUser = userResponse.User;
            }

            // Display errors if neither was given.
            if (anilistUser == null)
            {
                if (!string.IsNullOrEmpty(anilistName))
                {
                    await RespondAsync(text: $"No Anilist user found! Make sure the account exists by navigating to `https://anilist.co/user/{anilistName}/`", ephemeral: true);
                }
                else
                {
                    await RespondAsync(text: $"No Anilist user found! Make sure the account exists by navigating to `https://anilist.co/user/{anilistId}/`", ephemeral: true);
                }

                return false;
            }

            // Prepare a new DiscordUser.
            var discordUser = new DiscordUser
            {
                DiscordId = Context.User.Id,
                Name = Context.User.Username,
                AnilistId = anilistUser.Id,
                AnilistName = anilistUser.Name
            };

            // Try to fetch user from DB to get the ID.
            var foundUser = await DatabaseUtility.GetInstance().GetSpecificUserAsync(Context.User.Id);
            if (foundUser != null)
            {
                // Set the ID.
                discordUser.Id = foundUser.Id;
            }

            return await DatabaseUtility.GetInstance().UpsertUserAsync(discordUser);
        }
    }
}
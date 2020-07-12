using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("setup")]
    [Alias("profile")]
    public class SetupModule : AbstractModule
    {
        /// <summary>
        /// Default catch for Setup telling user what to do next.
        /// </summary>
        [Command]
        [Summary("Start the setup process.")]
        public async Task SetupAsync()
        {
            await Context.Channel.SendMessageAsync("Annie May at your service, your friendly e-neighbourhood Anilist bot!\n\n" +
                "Register your anilist using `setup anilist <username/id>` or update using `setup update <username/id>`.");
        }

        /// <summary>
        /// Setup to register a DiscordUser with their Anilist username.
        /// </summary>
        /// <param name="anilistName">String indicating their Anilist username.</param>
        [Command("anilist")]
        [Summary("Have a user add or update their anilist to the database using username.")]
        [Alias("edit", "update")]
        public async Task SetupAnilistAsync([Remainder] string anilistName)
        {
            if (await UpsertAnilistUser(anilistName, 0))
            {
                await Context.Message.AddReactionAsync(new Emoji("\u2611"));
            }
        }

        /// <summary>
        /// Setup to register a DiscordUser with their Anilist ID.
        /// </summary>
        /// <param name="anilistId">Number indicating their Anilist ID.</param>
        [Command("anilist")]
        [Summary("Have a user add or update their anilist to the database using id.")]
        [Alias("edit", "update")]
        public async Task SetupAnilistAsync([Remainder] int anilistId)
        {
            if (await UpsertAnilistUser(null, anilistId))
            {
                await Context.Message.AddReactionAsync(new Emoji("\u2611"));
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
                    await Context.Channel.SendMessageAsync($"No Anilist user found! Make sure the account exists by navigating to `https://anilist.co/user/{anilistName}/`");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"No Anilist user found! Make sure the account exists by navigating to `https://anilist.co/user/{anilistId}/`");
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
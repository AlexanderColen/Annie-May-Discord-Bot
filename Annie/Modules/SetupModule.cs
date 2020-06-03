using AnnieMayDiscordBot.Models;
using Discord;
using Discord.Commands;
using MongoDB.Driver;
using System.Threading.Tasks;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels;

namespace AnnieMayDiscordBot.Modules
{
    [Group("setup")]
    [Alias("profile")]
    public class SetupModule : AbstractModule
    {
        [Command()]
        [Summary("Start the setup process.")]
        public async Task SetupAsync()
        {
            await Context.Channel.SendMessageAsync("Hi there, I am Annie May, your friendly e-neighbourhood Anilist bot!");
            await Context.Channel.SendMessageAsync("Register your anilist using `setup anilist <username/id>`.");
        }

        [Command("anilist")]
        [Summary("Have a user add their anilist to the database using username.")]
        public async Task SetupAnilistAsync([Remainder] string anilistName)
        {
            if (!await CheckExistingAnilist())
            {
                if (await AddAnilistUser(anilistName, 0))
                {
                    await Context.Channel.SendMessageAsync("Successfully added your Anilist account!");
                }
            }
        }

        [Command("anilist")]
        [Summary("Have a user add their anilist to the database using id.")]
        public async Task SetupAnilistAsync([Remainder] int anilistId)
        {
            if (!await CheckExistingAnilist())
            {
                if (await AddAnilistUser(null, anilistId))
                {
                    await Context.Channel.SendMessageAsync("Successfully added your Anilist account!");
                }
            }
        }

        [Command("update")]
        [Summary("Have a user edit their anilist in the database using username.")]
        [Alias("edit")]
        public async Task SetupEditAsync([Remainder] string anilistName)
        {
            if (await CheckExistingUser())
            {
                if (await UpdateAnilistUser(anilistName, 0))
                {
                    await Context.Channel.SendMessageAsync("Successfully edited your database record!");
                }
            }
        }

        [Command("update")]
        [Summary("Have a user edit their anilist in the database using id.")]
        [Alias("edit")]
        public async Task SetupEditAsync([Remainder] int anilistId)
        {
            if (await CheckExistingUser())
            {
                if (await UpdateAnilistUser(null, anilistId))
                {
                    await Context.Channel.SendMessageAsync("Successfully edited your database record!");
                }
            }
        }

        /// <summary>
        /// Checks whether a User exists in the database.
        /// </summary>
        /// <returns>True if the User exists, otherwise false.</returns>
        private async Task<bool> CheckExistingUser()
        {
            IMongoDatabase db = _dbClient.GetDatabase("AnnieMayBot");
            var usersCollection = db.GetCollection<DiscordUser>("users");
            var filter = Builders<DiscordUser>.Filter.Eq("discordId", Context.User.Id);
            var users = usersCollection.Find(filter).ToList();

            if (users.Count == 0)
            {
                await Context.User.SendMessageAsync("You're not in my records... Please make sure to setup first using `setup anilist <username/id>`.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a User's Anilist already exists in the database.
        /// </summary>
        /// <param name="discordId">The User's Discord ID.</param>
        /// <returns>True if the User already has their anilist username/id set in the database, otherwise false.</returns>
        private async Task<bool> CheckExistingAnilist()
        {
            IMongoDatabase db = _dbClient.GetDatabase("AnnieMayBot");
            var usersCollection = db.GetCollection<DiscordUser>("users");
            var filter = Builders<DiscordUser>.Filter.Eq("discordId", Context.User.Id);
            var users = usersCollection.Find(filter).ToList();

            if (users.Count > 0 && users[0].anilistName != null && users[0].anilistId != 0)
            {
                await Context.User.SendMessageAsync($"Your Anilist account is already registered in the database ({users[0].anilistName}). You may update this using `setup update <username/id>`.");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a new DiscordUser to the database using their anilist info.
        /// </summary>
        /// <param name="discordId">The User's Discord ID.</param>
        /// <returns>True if the new DiscordUser is put in the database, otherwise false.</returns>
        private async Task<bool> AddAnilistUser(string anilistName, int anilistId)
        {
            User user = null;
            // Check whether to search for the user using their name or their id.
            if (anilistName != null)
            {
                UserResponse userResponse = await _aniListFetcher.FindUserAsync(anilistName);
                user = userResponse.user;
            }
            else if (anilistId != 0)
            {
                UserResponse userResponse = await _aniListFetcher.FindUserAsync(anilistId);
                user = userResponse.user;
            }

            if (user == null)
            {
                if (anilistName != null)
                {
                    await Context.User.SendMessageAsync($"No Anilist user found! Make sure the account exists by navigating to `https://anilist.co/user/{anilistName}/`");
                }
                else
                {
                    await Context.User.SendMessageAsync($"No Anilist user found! Make sure the account exists by navigating to `https://anilist.co/user/{anilistId}/`");
                }
                
                return false;
            }

            IMongoDatabase db = _dbClient.GetDatabase("AnnieMayBot");
            var usersCollection = db.GetCollection<DiscordUser>("users");
            DiscordUser discordUser = new DiscordUser
            {
                discordId = Context.User.Id,
                name = Context.User.Username,
                anilistId = user.id,
                anilistName = user.name
            };

            await usersCollection.InsertOneAsync(discordUser);

            return true;
        }

        private async Task<bool> UpdateAnilistUser(string anilistName, int anilistId)
        {
            User user = null;
            if (anilistName != null)
            {
                UserResponse userResponse = await _aniListFetcher.FindUserAsync(anilistName);
                user = userResponse.user;
            }
            else if (anilistId != 0)
            {
                UserResponse userResponse = await _aniListFetcher.FindUserAsync(anilistId);
                user = userResponse.user;
            }

            if (user == null)
            {
                if (anilistName != null)
                {
                    await Context.User.SendMessageAsync($"No Anilist user found! Make sure the account exists by navigating to `https://anilist.co/user/{anilistName}/`");
                }
                else
                {
                    await Context.User.SendMessageAsync($"No Anilist user found! Make sure the account exists by navigating to `https://anilist.co/user/{anilistId}/`");
                }

                return false;
            }

            IMongoDatabase db = _dbClient.GetDatabase("AnnieMayBot");
            var usersCollection = db.GetCollection<DiscordUser>("users");
            var filter = Builders<DiscordUser>.Filter.Eq("discordId", Context.User.Id);
            var update = Builders<DiscordUser>.Update.Set("anilistName", user.name)
                                                     .Set("anilistId", user.id);

            await usersCollection.UpdateOneAsync(filter, update);

            return true;
        }
    }
}

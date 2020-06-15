using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord;
using Discord.Commands;
using MongoDB.Driver;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class ListModule : AbstractModule
    {
        /// <summary>
        /// Display the User's Anilist information.
        /// </summary>
        [Command("user")]
        [Summary("Find a user's statistics without any parameters.")]
        [Alias("list", "userlist", "anilist")]
        public async Task GetUserAniListAsync()
        {
            long userId = FetchAnilistIdFromDatabase(Context.User.Id);
            if (userId == -1)
            {
                await Context.Channel.SendMessageAsync("You're not in my records... Please make sure to setup first using `setup anilist <username/id>`.");
                return;
            }

            try
            {
                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(userId);
                await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.User));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }

        /// <summary>
        /// Display a specific User's Anilist information using an Anilist username.
        /// </summary>
        /// <param name="username">An Anilist username.</param>
        [Command("user")]
        [Summary("Find a user's statistics using their username.")]
        [Alias("list", "userlist", "anilist")]
        public async Task GetUserAniListAsync([Remainder] string username)
        {
            try
            {
                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(username);
                await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.User));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }

        /// <summary>
        /// Display a specific User's Anilist information using an Anilist User ID.
        /// </summary>
        /// <param name="userId">An Anilist User ID.</param>
        [Command("user")]
        [Summary("Find a user's statistics using their id.")]
        [Alias("list", "userlist", "anilist")]
        public async Task GetUserAniListAsync([Remainder] long userId)
        {
            // Check if the given int parameter is a Discord User ID (18 characters long).
            if (userId.ToString().Length == 17)
            {
                long foundId = FetchAnilistIdFromDatabase((ulong)userId);
                if (foundId == -1)
                {
                    await ReplyAsync("This filthy weeb isn't in the database.");
                    return;
                }

                userId = foundId;
            }

            try
            {
                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(userId);
                await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.User));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }

        /// <summary>
        /// Display a specific User's Anilist information using a tagged Discord User.
        /// </summary>
        /// <param name="user">A tagged Discord User.</param>
        [Command("user")]
        [Summary("Find a user's statistics using their username.")]
        [Alias("list", "userlist", "anilist")]
        public async Task GetUserAniListAsync([Remainder] IUser user)
        {
            try
            {
                long userId = FetchAnilistIdFromDatabase(user.Id);
                if (userId == -1)
                {
                    await ReplyAsync("This filthy weeb isn't in the database.");
                    return;
                }

                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(userId);
                await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.User));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }

        /// <summary>
        /// Fetch the Anilist ID from the database by looking for the User with the Discord ID.
        /// </summary>
        /// <param name="discordId">The Discord ID of the User.</param>
        /// <returns>The Anilist ID if found, otherwise -1.</returns>
        private long FetchAnilistIdFromDatabase(ulong discordId)
        {
            // Look the user's anilist ID up in the database, if found replace the userId parameter with that.
            IMongoDatabase db = _dbClient.GetDatabase("AnnieMayBot");
            var usersCollection = db.GetCollection<DiscordUser>("users");
            var filter = Builders<DiscordUser>.Filter.Eq("discordId", discordId);
            var users = usersCollection.Find(filter).ToList();

            // Return -1 if the user wasn't found.
            if (users.Count == 0)
            {
                return -1;
            }

            return users[0].anilistId;
        }
    }
}
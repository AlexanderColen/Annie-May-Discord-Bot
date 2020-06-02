using System.Net.Http;
using System.Threading.Tasks;
using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.ResponseModels;
using Discord.Commands;
using MongoDB.Driver;

namespace AnnieMayDiscordBot.Modules
{
    public class ListModule : AbstractModule
    {
        [Command("user")]
        [Summary("Find a user's statistics using their username.")]
        [Alias("list", "userlist")]
        public async Task GetAnimeListAsync([Remainder] string username)
        {
            try
            {
                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(username);
                await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.user));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }

        [Command("user")]
        [Summary("Find a user's statistics using their id.")]
        [Alias("list", "userlist")]
        public async Task GetAnimeListAsync([Remainder] long userId)
        {
            // Check if the given int parameter is a Discord User ID (18 characters long).
            if (userId.ToString().Length == 18)
            {
                // Look the user's anilist ID up in the database, if found replace the userId parameter with that.
                IMongoDatabase db = _dbClient.GetDatabase("AnnieMayBot");
                var usersCollection = db.GetCollection<DiscordUser>("users");
                var filter = Builders<DiscordUser>.Filter.Eq("discordId", userId);
                var users = usersCollection.Find(filter).ToList();

                if (users.Count == 0)
                {
                    // If the user doesn't exist, inform the requester and return out of the method.
                    await ReplyAsync("This filthy weeb isn't in the database.");
                    return;
                }

                userId = users[0].anilistId;
            }

            try
            {
                UserResponse userResponse = await _aniListFetcher.FindUserStatisticsAsync(userId);
                await ReplyAsync("", false, _embedUtility.BuildUserEmbed(userResponse.user));
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("Sorry, I could not find this Anilist user.");
            }
        }
    }
}

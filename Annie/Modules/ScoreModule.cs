using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("scores")]
    [Alias("scoredistribution", "userscores")]
    public class ScoreModule : AbstractModule
    {
        /// <summary>
        /// Get a compiled list of the scored Media for the User. No arguments defaults to Anime.
        /// </summary>
        [Command]
        [Summary("Get a compiled list of the scored Media for the User.")]
        public async Task GetUserScoresAsync()
        {
            var user = await _databaseUtility.GetSpecificUserAsync(Context.User.Id);
            if (user == null)
            {
                await ReplyAsync($"Wait who dis? Please register your Anilist using `{Properties.Resources.PREFIX}setup anilist <USERNAME/ID>`");
                return;
            }
            MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(user.anilistId, MediaType.Anime.ToString());
            await ReplyAsync(null, false, _embedUtility.BuildScoresEmbed(response.MediaListCollection, MediaType.Anime));
        }

        /// <summary>
        /// Get a compiled list of the scored Media for the specified Anilist username without parameters.
        /// </summary>
        /// <param name="username">An Anilist username.</param>s
        [Command]
        [Summary("Get a compiled list of the scored Media for the specified Anilist username without parameters.")]
        public async Task GetUserScoresAsync(string username)
        {
            MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(username, MediaType.Anime.ToString());
            await ReplyAsync(null, false, _embedUtility.BuildScoresEmbed(response.MediaListCollection, MediaType.Anime));
        }

        /// <summary>
        /// Get a compiled list of the scored Media for the specified Anilist userId without parameters.
        /// </summary>
        /// <param name="userId">An Anilist User ID.</param>s
        [Command]
        [Summary("Get a compiled list of the scored Media for the User without parameters.")]
        public async Task GetUserScoresAsync(long userId)
        {
            // Check if the given long parameter is a Discord User ID (17-18 characters long).
            if (userId.ToString().Length >= 17)
            {
                var user = await _databaseUtility.GetSpecificUserAsync((ulong)userId);
                if (user == null)
                {
                    await ReplyAsync("This filthy weeb isn't in the database.");
                    return;
                }
                // Overwrite the userId with the found Anilist ID.
                userId = user.anilistId;
            }
            MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(userId, MediaType.Anime.ToString());
            await ReplyAsync(null, false, _embedUtility.BuildScoresEmbed(response.MediaListCollection, MediaType.Anime));
        }

        /// <summary>
        /// Get a compiled list of the scored Media for the Discord User without parameters.
        /// </summary>
        /// <param name="user">The tagged Discord User.</param>
        [Command]
        [Summary("Get a compiled list of the scored Media for the Discord User without parameters.")]
        public async Task GetUserScoresAsync(IUser user)
        {
            var discordUser = await _databaseUtility.GetSpecificUserAsync(user.Id);
            if (discordUser == null)
            {
                await ReplyAsync($"Wait who dat? Please have them register their Anilist using `{Properties.Resources.PREFIX}setup anilist <USERNAME/ID>`");
                return;
            }
            MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(discordUser.anilistId, MediaType.Anime.ToString());
            await ReplyAsync(null, false, _embedUtility.BuildScoresEmbed(response.MediaListCollection, MediaType.Anime));
        }
    }
}
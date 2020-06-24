using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord;
using Discord.Commands;
using System;
using System.Text.RegularExpressions;
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
            MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(user.AnilistId, MediaType.Anime.ToString());
            await ReplyAsync(null, false, _embedUtility.BuildScoresEmbed(response.MediaListCollection, MediaType.Anime));
        }

        /// <summary>
        /// Get a compiled list of the scored Media for the specified Anilist username without parameters.
        /// </summary>
        /// <param name="username">An Anilist username.</param>
        [Command]
        [Summary("Get a compiled list of the scored Media for the specified Anilist username without parameters.")]
        public async Task GetUserScoresAsync(string username)
        {
            MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(username, MediaType.Anime.ToString());
            await ReplyAsync(null, false, _embedUtility.BuildScoresEmbed(response.MediaListCollection, MediaType.Anime));
        }

        /// <summary>
        /// Get a compiled list of the scored Media for the specified Discord or Anilist userId without parameters.
        /// </summary>
        /// <param name="userId">An Anilist User ID.</param>
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
                userId = user.AnilistId;
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

            MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(discordUser.AnilistId, MediaType.Anime.ToString());
            await ReplyAsync(null, false, _embedUtility.BuildScoresEmbed(response.MediaListCollection, MediaType.Anime));
        }

        /// <summary>
        /// Get a compiled list of the scored Media for the specified Anilist username with parameters.
        /// </summary>
        /// <param name="username">An Anilist username.</param>
        /// <param name="parameters">The parameters attached to the request.</param>
        [Command]
        [Summary("Get a compiled list of the scored Media for the specified Anilist username with parameters.")]
        public async Task GetUserScoresAsync(string username, [Remainder] string parameters)
        {
            var tuple = ParseParameters(parameters);
            // No bounds specified.
            if (tuple.Item2 == 0 && tuple.Item3 == 100)
            {
                MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(username, tuple.Item1.ToString());
                await ReplyAsync(null, false, _embedUtility.BuildScoresEmbed(response.MediaListCollection, tuple.Item1));
            }
            // Bounds specified.
            else
            {
                MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(username, tuple.Item1.ToString());
                await ReplyAsync(null, false, _embedUtility.BuildCustomScoresEmbed(response.MediaListCollection, tuple.Item1, tuple.Item2, tuple.Item3));
            }
        }

        /// <summary>
        /// Get a compiled list of the scored Media for the specified Discord or Anilist userId with parameters.
        /// </summary>
        /// <param name="userId">An Anilist User ID.</param>
        /// <param name="parameters">The parameters attached to the request.</param>
        [Command]
        [Summary("Get a compiled list of the scored Media for the specified Discord or Anilist userId with parameters.")]
        public async Task GetUserScoresAsync(long userId, [Remainder] string parameters)
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
                userId = user.AnilistId;
            }

            var tuple = ParseParameters(parameters);
            // No bounds specified.
            if (tuple.Item2 == 0 && tuple.Item3 == 100)
            {
                MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(userId, tuple.Item1.ToString());
                await ReplyAsync(null, false, _embedUtility.BuildScoresEmbed(response.MediaListCollection, tuple.Item1));
            }
            // Bounds specified.
            else
            {
                MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(userId, tuple.Item1.ToString());
                await ReplyAsync(null, false, _embedUtility.BuildCustomScoresEmbed(response.MediaListCollection, tuple.Item1, tuple.Item2, tuple.Item3));
            }
        }

        /// <summary>
        /// Get a compiled list of the scored Media for the Discord User with parameters.
        /// </summary>
        /// <param name="user">The tagged Discord User.</param>
        /// <param name="parameters">The parameters attached to the request.</param>
        [Command]
        [Summary("Get a compiled list of the scored Media for the Discord User with parameters.")]
        public async Task GetUserScoresAsync(IUser user, [Remainder] string parameters)
        {
            var discordUser = await _databaseUtility.GetSpecificUserAsync(user.Id);
            if (discordUser == null)
            {
                await ReplyAsync($"Wait who dat? Please have them register their Anilist using `{Properties.Resources.PREFIX}setup anilist <USERNAME/ID>`");
                return;
            }

            var tuple = ParseParameters(parameters);
            // No bounds specified.
            if (tuple.Item2 == 0 && tuple.Item3 == 100)
            {
                MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(discordUser.AnilistId, tuple.Item1.ToString());
                await ReplyAsync(null, false, _embedUtility.BuildScoresEmbed(response.MediaListCollection, tuple.Item1));
            }
            // Bounds specified.
            else
            {
                MediaListCollectionResponse response = await _aniListFetcher.FindUserListScoresAsync(discordUser.AnilistId, tuple.Item1.ToString());
                await ReplyAsync(null, false, _embedUtility.BuildCustomScoresEmbed(response.MediaListCollection, tuple.Item1, tuple.Item2, tuple.Item3));
            }
        }

        /// <summary>
        /// Parse parameters that are added to the command.
        /// </summary>
        /// <param name="parameters">The parameters attached to the request.</param>
        /// <returns>A Tuple containing a MediaType string and lower and upper bounds ints.</returns>
        private Tuple<MediaType, int, int> ParseParameters(string parameters)
        {
            // Lower parameters.
            parameters = parameters.ToLower();
            // Check for anime/manga parameters.
            bool hasAnime = parameters.Contains("anime");
            bool hasManga = parameters.Contains("manga");

            // Both anime and manga in the parameters is not allowed.
            if (hasAnime && hasManga)
            {
                ReplyAsync("Failed to parse parameters. There can only be zero or one anime or manga parameter.");
                return null;
            }

            // Remove anime and manga parameters.
            parameters = parameters.Replace("anime", "").Replace("manga", "").Trim();

            // Check for <, >, =, >= or <= followed by a number.
            var matches = Regex.Matches(parameters, "((>=|<=|>|<|=){1}\\d+)");

            // Set bounds based on appropriate parameters.
            var bounds = Tuple.Create(0, 100);
            if (matches.Count > 0)
            {
                var minMaxTuple = CalculateBounds(matches);
                if (minMaxTuple != null)
                {
                    bounds = minMaxTuple;
                }
            }

            // If no manga was specified, it means either nothing or anime was given, defaulting to anime.
            if (!hasManga)
            {
                return Tuple.Create(MediaType.Anime, bounds.Item1, bounds.Item2);
            }
            // Else manga was the choise.
            return Tuple.Create(MediaType.Manga, bounds.Item1, bounds.Item2);
        }

        /// <summary>
        /// Handle the matches to determine the scoring bounds.
        /// </summary>
        /// <param name="matches">The Regex matches fo</param>
        /// <returns>A Tuple containing the lower and upper bounds ints from the matches.</returns>
        private Tuple<int, int> CalculateBounds(MatchCollection matches)
        {
            int lowerBounds = 0;
            int upperBounds = 100;
            foreach (var match in matches)
            {
                Regex re = new Regex("([><=]+)(\\d+)");
                Match result = re.Match(match.ToString());

                string alphaPart = result.Groups[1].Value;
                int numberPart = int.Parse(result.Groups[2].Value);
                // More than.
                if (alphaPart.Equals(">"))
                {
                    // Check if possible with current bounds.
                    if (numberPart >= lowerBounds && numberPart <= upperBounds)
                    {
                        // More than means it cannot be the number itself, but can be 1 more.
                        lowerBounds = numberPart + 1;
                    }
                }
                // Less than.
                else if (alphaPart.Equals("<"))
                {
                    // Check if possible with current bounds.
                    if (numberPart >= lowerBounds && numberPart <= upperBounds)
                    {
                        // Less than means it cannot be the number itself, but can be 1 less.
                        upperBounds = numberPart - 1;
                    }
                }
                // Equals.
                else if (alphaPart.Equals("="))
                {
                    // Check if possible with current bounds.
                    if (numberPart >= lowerBounds && numberPart <= upperBounds)
                    {
                        // Equals is self-explanatory.
                        upperBounds = numberPart;
                        lowerBounds = numberPart;
                    }
                }
                // More than or equals.
                else if (alphaPart.Equals(">="))
                {
                    // Check if possible with current bounds.
                    if (numberPart >= lowerBounds && numberPart <= upperBounds)
                    {
                        // Less than means lower bounds needs to be adjusted.
                        lowerBounds = numberPart;
                    }
                }
                // Less than or equals.
                else if (alphaPart.Equals("<="))
                {
                    // Check if possible with current bounds.
                    if (numberPart >= lowerBounds && numberPart <= upperBounds)
                    {
                        // Less than means upper bounds needs to be adjusted.
                        upperBounds = numberPart;
                    }
                }
                // Anything else is not allowed so reply with an error.
                else
                {
                    ReplyAsync("Failed to parse parameters. There can only be `>`, `<`, `=`, `>=`, `<=` comparator parameters.");
                    return null;
                }
            }

            // Check if lower and upper bounds are possible together.
            if (lowerBounds > upperBounds)
            {
                ReplyAsync("Failed to parse parameters. Your lower bounds cannot be higher than your upper bounds.");
                return null;
            }

            return Tuple.Create(lowerBounds, upperBounds);
        }
    }
}
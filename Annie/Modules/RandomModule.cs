using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.Properties;
using AnnieMayDiscordBot.Utility;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class RandomModule : AbstractModule
    {
        /// <summary>
        /// Tell the user the random options available
        /// </summary>
        [Command("random")]
        [Summary("Tell the user the random options available.")]
        public async Task RandomAsync()
        {
            await ReplyAsync($"I offer the following random services:\n" +
                             $"Integer rolling: `{Resources.PREFIX}random roll` _(You can supply minimum and maximum integers)_\n" +
                             $"Coinflips: `{Resources.PREFIX}random coinflip`\n" +
                             $"Planned reccomendations: `{Resources.PREFIX}random ptw` _(A connected Anilist account is required.)_\n\n" +
                             $"You can check `{Resources.PREFIX}help random` for more details.", false);
        }

        /// <summary>
        /// Handle the random choice.
        /// </summary>
        /// <param name="parameters">The parameters that were added to specify what random to get.</param>
        [Command("random")]
        [Summary("Handle the random choice.")]
        public async Task RandomAsync([Remainder] string parameters)
        {
            string[] arguments = parameters.Split(' ');
            // Execute differently based on second argument.
            switch (arguments[0])
            {
                case "roll":
                    // 1 argument means no min/max provided.
                    if (arguments.Length == 1)
                    {
                        await RollAsync();
                    }
                    else
                    {
                        // 2 arguments means max provided.
                        if (arguments.Length == 2)
                        {
                            // Try to parse to int, reply with failure if necessary.
                            if (!int.TryParse(arguments[1], out var max))
                            {
                                await ReplyAsync("Please provide an integer to specify the max after `roll`.\n_For example `roll 6`_", false);
                                return;
                            }
                            await RollUpToAsync(max);
                        }
                        // 3 arguments means min and max provided.
                        else if (arguments.Length == 3)
                        {
                            // Try to parse to int, reply with failure if necessary.
                            bool validMin = int.TryParse(arguments[1], out var min);
                            bool validMax = int.TryParse(arguments[2], out var max);
                            if (!validMin || !validMax)
                            {
                                await ReplyAsync("Please provide two integers to specify min and max after `roll`.\n_For example `roll 10 100`_", false);
                                return;
                            }
                            await RollBetweenAsync(min, max);
                        }
                        else
                        {
                            await ReplyAsync("A maximum of two integers are allowed after `roll`.", false);
                        }
                    }
                    break;
                case "coinflip":
                    await CoinflipAsync();
                    break;
                case "planned":
                    await PlannedAsync();
                    break;
                // Extra alias catching.
                case "die":
                    goto case "roll";
                case "dice":
                    goto case "roll";
                case "coin":
                    goto case "coinflip";
                case "flip":
                    goto case "coinflip";
                case "ptw":
                    goto case "planned";
                case "ptr":
                    goto case "planned";
                case "plan":
                    goto case "planned";
                case "plantowatch":
                    goto case "planned";
                case "plantoread":
                    goto case "planned";
                case "planning":
                    goto case "planned";
                default:
                    await ReplyAsync($"{arguments[0]} is not _(yet)_ supported in this command.", false);
                    break;
            }
        }

        /// <summary>
        /// Roll a die.
        /// </summary>
        [Command("roll")]
        [Summary("Roll a die.")]
        [Alias("die", "dice")]
        public async Task RollAsync()
        {
            Random random = new Random((int)DateTime.UtcNow.Ticks);
            await ReplyAsync($"神様 has blessed you with a {random.Next()}.", false);
        }

        /// <summary>
        /// Roll a die up to a custom maximum.
        /// <param name="max">The highest number the roll can be.</param>
        /// </summary>
        [Command("roll")]
        [Summary("Roll a die up to a custom maximum.")]
        [Alias("die", "dice")]
        public async Task RollUpToAsync(int max)
        {
            Random random = new Random((int)DateTime.UtcNow.Ticks);
            await ReplyAsync($"神様 has blessed you with a {random.Next(1, max+1)}.", false);
        }

        /// <summary>
        /// Roll a die inclusive between a custom minimum and maximum.
        /// <param name="min">The lowest number the roll can be.</param>
        /// <param name="max">The highest number the roll can be.</param>
        /// </summary>
        [Command("roll")]
        [Summary("Roll a die inclusive between a custom minimum and maximum.")]
        [Alias("die", "dice")]
        public async Task RollBetweenAsync(int min, int max)
        {
            if (min > max)
            {
                await ReplyAsync("The minimum cannot be higher than the maximum.");
                return;
            }

            Random random = new Random((int)DateTime.UtcNow.Ticks);
            await ReplyAsync($"神様 has blessed you with a {random.Next(min, max+1)}.", false);
        }

        /// <summary>
        /// Flip a coin at random.
        /// </summary>
        [Command("coinflip")]
        [Summary("Flip a coin at random.")]
        [Alias("coin", "flip")]
        public async Task CoinflipAsync()
        {
            Random random = new Random((int)DateTime.UtcNow.Ticks);
            var coin = Resources.coin_heads;
            var result = "heads";
            // Even is heads, odd is tails.
            if (random.Next(0, 100) % 2 == 1)
            {
                coin = Resources.coin_tails;
                result = "tails";
            }

            var ms = new MemoryStream();
            coin.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(ms, $"{result}.png", null);
        }

        /// <summary>
        /// Fetch a random Planned media entry for the user.
        /// </summary>
        [Command("planned")]
        [Summary("Fetch a random Planned media entry for the user.")]
        [Alias("plantowatch", "plantoread", "ptw", "ptr", "plan", "planning")]
        public async Task PlannedAsync()
        {
            var user = await DatabaseUtility.GetInstance().GetSpecificUserAsync(Context.User.Id);
            if (user == null)
            {
                await ReplyAsync("You need to tell me your Anilist before I can recommend you stuff!\n" +
                    "You can do this using the `setup anilist <ID/USERNAME>` command.", false);
                return;
            }

            var planned = new List<MediaList>();
            var animeList = await _aniListFetcher.FindPlannedUserList(user.AnilistId, MediaType.Anime.ToString());
            var mangaList = await _aniListFetcher.FindPlannedUserList(user.AnilistId, MediaType.Manga.ToString());

            if (animeList.MediaListCollection.Lists.Count > 0)
            {
                planned.AddRange(animeList.MediaListCollection.Lists[0].Entries);
            }

            if (mangaList.MediaListCollection.Lists.Count > 0)
            {
                planned.AddRange(mangaList.MediaListCollection.Lists[0].Entries);
            }

            if (planned.Count == 0)
            {
                await ReplyAsync("You might want to add entries to Planning before asking me to choose one.", false);
                return;
            }

            Random rand = new Random((int)DateTime.UtcNow.Ticks);

            var chosenOne = planned[rand.Next(planned.Count)];

            var consume = chosenOne.Media.Type == MediaType.Anime ? "watch" : "read";

            List<string> recommendMessages = new List<string>
            {
                $"I wholeheartedly recommend that you {consume} this one. It's one of my clone's favourites.",
                $"You totally should {consume} this. I heard that it's one of the best out there.",
                $"I cannot believe that you never got to {consume} this...",
                $"Why has nobody told you to {consume} this? It has such a reputation for a reason!",
                $"The favourite count on this one is wrong. It's going to have +1 after you finish {consume}ing it.",
                $"Go {consume} this right now. It's a classic for a reason.",
                "What have you been wasting your time on instead of this? Disgraceful.",
                $"If you {consume} this, I will fullfil one of your deepest desires. \uD83D\uDE18",
                $"You better {consume} this one, or I'm sending one of my clones after you.",
                "This is the next best thing after sliced bread and Shidou."
            };

            await ReplyAsync(recommendMessages[rand.Next(recommendMessages.Count)], false, _embedUtility.BuildAnilistMediaEmbed(chosenOne.Media));
        }
    }
}
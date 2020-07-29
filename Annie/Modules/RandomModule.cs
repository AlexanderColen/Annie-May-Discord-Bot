using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Properties;
using Discord.Commands;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class RandomModule : ModuleBase<CustomCommandContext>
    {
        /// <summary>
        /// Handle the random choice.
        /// </summary>
        /// <param name="parameters">The parameters that were added to specify what random to get.</param>
        [Command("random")]
        [Summary("Handle the random choice.")]
        public async Task Random([Remainder] string parameters)
        {
            string[] arguments = parameters.Split(' ');
            // Execute differently based on second argument.
            switch (arguments[0])
            {
                case "roll":
                    // 1 argument means no min/max provided.
                    if (arguments.Length == 1)
                    {
                        await Roll();
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
                            await RollUpTo(max);
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
                            await RollBetween(min, max);
                        }
                        else
                        {
                            await ReplyAsync("A maximum of two integers are allowed after `roll`.", false);
                        }
                    }
                    break;
                case "coinflip":
                    await Coinflip();
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
        public async Task Roll()
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
        public async Task RollUpTo(int max)
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
        public async Task RollBetween(int min, int max)
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
        public async Task Coinflip()
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
    }
}
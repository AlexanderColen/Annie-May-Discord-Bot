using AnnieMayDiscordBot.Properties;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord.Interactions;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class FluffModule : AbstractInteractionModule
    {

        /// <summary>
        /// Reply to ping with athe bot's latency.
        /// </summary>
        [SlashCommand("ping", "Pings the bot and returns its latency.")]
        public async Task PingResponseAsync()
        {
            await RespondAsync(text: $":ping_pong: Pong! It took me {Context.Client.Latency}ms to respond to you!", isTTS: false, ephemeral: true);
        }

        /// <summary>
        /// Reply to catface with an image.
        /// </summary>
        [SlashCommand("catface", "Respond with the catface image.")]
        public async Task CatFaceResponseAsync()
        {
            await Context.Channel.SendFileAsync(new MemoryStream(Resources.catface), "catface.png",
                null);
        }

        /// <summary>
        /// Reply to eva with an image.
        /// </summary>
        [SlashCommand("eva", "Respond with the Eva image.")]
        public async Task EvaResponseAsync()
        {
            await Context.Channel.SendFileAsync(new MemoryStream(Resources.eva), "eva.png",
                null);
        }

        /// <summary>
        /// Reply to taste with an image.
        /// </summary>
        [SlashCommand("taste", "Respond with the Taste image.")]
        public async Task TasteResponseAsync()
        {
            await Context.Channel.SendFileAsync(new MemoryStream(Resources.taste), "taste.png",
                null);
        }

        /// <summary>
        /// Reply to andre with an image.
        /// </summary>
        [SlashCommand("andre", "Respond with the Andre image.")]
        public async Task AndreResponseAsync()
        {
            var andre = Resources.andre;
            var ms = new MemoryStream();
            andre.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(ms, $"andre.png", null);
        }

        /// <summary>
        /// Reply to waifu with the best girl.
        /// </summary>
        [SlashCommand("waifu", "Respond with a best girl.")]
        public async Task WaifuAsync()
        {
            CharacterResponse characterResponse = await _aniListFetcher.FindCharacterAsync(70069);
            await RespondAsync(isTTS: false, embed: _embedUtility.BuildAnilistCharacterEmbed(characterResponse.Character));
        }

        /// <summary>
        /// Have Annie May say a special message.
        /// </summary>
        [SlashCommand("draw", "Have Annie May say a special message.")]
        public async Task DrawMessageAsync(string message)
        {
            // Load image.
            Bitmap bitmap = Resources.AnnieMaySign;
            using Graphics graphics = Graphics.FromImage(bitmap);
            var rotation = 16.5f;
            var maxLength = 220;

            // Start calculations for string splitting & positioning.
            using Font arialFont = new Font("Arial", 20);
            var stringSize = graphics.MeasureString(message, arialFont);

            // Max message size would be length of 230 * max lines.
            if (stringSize.Width > maxLength * 3)
            {
                await SendTooBigMessage();
                return;
            }

            List<string> lines = new List<string>
            {
                message
            };

            // Split message into multiple lines if length is more than 230.
            if (stringSize.Width > maxLength)
            {
                lines = new List<string>();
                // Split into new strings up until width.
                var str = "";
                foreach (var c in message)
                {
                    if (!str.Equals("") && graphics.MeasureString(str, arialFont).Width > maxLength)
                    {
                        lines.Add(str);
                        str = "";
                    }
                    str += c;
                }

                // Make sure to add the residue.
                if (!str.Equals(""))
                {
                    lines.Add(str);
                }

                // Maximum of 3 lines allowed.
                if (lines.Count > 3)
                {
                    await SendTooBigMessage();
                    return;
                }
            }

            // Rotate whole image.
            graphics.RotateTransform(-rotation);

            // Draw every line.
            for (int i = 0; i < lines.Count; i++)
            {
                var lineSize = graphics.MeasureString(lines[i], arialFont);
                var height = 130f - lineSize.Height / 2;
                if (i == 0)
                {
                    height -= 35;
                }
                else if (i == 2)
                {
                    height += 35;
                }
                PointF drawLocation = new PointF(90f - lineSize.Width / 2, height);
                graphics.DrawString(lines[i], arialFont, Brushes.DarkRed, drawLocation);
            }

            // Rotate the image back.
            graphics.RotateTransform(rotation);

            // Save and send the image.
            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(ms, "drawn_message.png", null);
        }

        /// <summary>
        /// Send a message that the current object is too big.
        /// </summary>
        private async Task SendTooBigMessage()
        {
            var emoji = "\uD83D\uDE33";
            // Find lewd emoji if possible.
            foreach (var emote in Context.Guild.Emotes)
            {
                if (emote.Name.Contains("lewd") && !emote.Animated)
                {
                    emoji = emote.ToString();
                    break;
                }
            }
            await Context.Channel.SendMessageAsync(text: $"Ooh-la-la aren't you unlucky. Sorry I cannot handle it that big. {emoji}");
        }
    }
}
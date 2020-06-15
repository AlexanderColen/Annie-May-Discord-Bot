using AnnieMayDiscordBot.Properties;
using Discord.Commands;
using System.IO;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class FluffModule : AbstractModule
    {
        /// <summary>
        /// Reply to :3 with an image.
        /// </summary>
        [Command(":3")]
        [Summary("Send an image response to the catface.")]
        public async Task CatFaceResponseAsync()
        {
            await Context.Channel.SendFileAsync(new MemoryStream(Resources.catface), "catface.png",
                null);
        }

        /// <summary>
        /// Reply to eva with an image.
        /// </summary>
        [Command("eva")]
        [Summary("Respond with an Eva image.")]
        public async Task EvaResponseAsync()
        {
            await Context.Channel.SendFileAsync(new MemoryStream(Resources.eva), "eva.png",
                null);
        }
    }
}
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnnieMayDiscordBot.Properties;
using AnnieMayDiscordBot.ResponseModels;
using Discord;
using Discord.Commands;

namespace AnnieMayDiscordBot.Modules
{
    public class ImageModule : AbstractModule
    {
        [Command(":3")]
        [Summary("Send an image response to the catface.")]
        public async Task CatFaceResponseAsync()
        {
            await Context.Channel.SendFileAsync(new MemoryStream(Resources.catface), "catface.png",
                embed: new EmbedBuilder { ImageUrl = "attachment://catface.png" }.Build());
        }
    }
}

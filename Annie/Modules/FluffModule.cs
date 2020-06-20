using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.Properties;
using AnnieMayDiscordBot.ResponseModels.Anilist;
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

        /// <summary>
        /// Reply to taste with an image.
        /// </summary>
        [Command("taste")]
        [Summary("Respond with a Taste image.")]
        [Alias("no taste")]
        public async Task TasteResponseAsync()
        {
            await Context.Channel.SendFileAsync(new MemoryStream(Resources.taste), "taste.png",
                null);
        }

        /// <summary>
        /// Reply to waifu with the best girl.
        /// </summary>
        [Command("waifu")]
        [Summary("Respond with a best girl.")]
        [Alias("best girl")]
        public async Task WaifuAsync()
        {
            CharacterResponse characterResponse = await _aniListFetcher.FindCharacterAsync(70069);
            await ReplyAsync("", false, _embedUtility.BuildAnilistCharacterEmbed(characterResponse.Character));
        }
    }
}
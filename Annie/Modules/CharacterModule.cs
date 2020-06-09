using AnnieMayDiscordBot.ResponseModels;
using Discord.Commands;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class CharacterModule : AbstractModule
    {
        [Command("character")]
        [Alias("waifu", "char")]
        [Summary("Find a character from AniList GraphQL based on string criteria.")]
        public async Task FindCharacterAsync([Remainder] string searchCriteria)
        {
            CharacterResponse characterResponse = await _aniListFetcher.FindCharacterAsync(searchCriteria);
            await ReplyAsync("", false, _embedUtility.BuildAnilistCharacterEmbed(characterResponse.character));
        }

        [Command("character")]
        [Alias("waifu", "char")]
        [Summary("Find a character from AniList GraphQL based on anilist character id.")]
        public async Task FindCharacterAsync([Remainder] int characterId)
        {
            CharacterResponse characterResponse = await _aniListFetcher.FindCharacterAsync(characterId);
            await ReplyAsync("", false, _embedUtility.BuildAnilistCharacterEmbed(characterResponse.character));
        }

        [Command("character?")]
        [Alias("waifu?", "char?")]
        [Summary("Find a character from AniList GraphQL based on string criteria including spoilers.")]
        public async Task FindCharacterSpoilersAsync([Remainder] string searchCriteria)
        {
            CharacterResponse characterResponse = await _aniListFetcher.FindCharacterAsync(searchCriteria);
            await ReplyAsync("", false, _embedUtility.BuildAnilistCharacterEmbed(characterResponse.character, true));
        }

        [Command("character?")]
        [Alias("waifu?", "char?")]
        [Summary("Find a character from AniList GraphQL based on anilist character id including spoilers.")]
        public async Task FindCharacterSpoilersAsync([Remainder] int characterId)
        {
            CharacterResponse characterResponse = await _aniListFetcher.FindCharacterAsync(characterId);
            await ReplyAsync("", false, _embedUtility.BuildAnilistCharacterEmbed(characterResponse.character, true));
        }
    }
}
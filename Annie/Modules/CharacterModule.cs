using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.AniList;
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
            PageResponse pageResponse = await _aniListFetcher.SearchCharactersAsync(searchCriteria);
            Character character = _levenshteinUtility.GetSingleBestCharacterResult(searchCriteria, pageResponse.Page.Characters);
            await ReplyAsync("", false, _embedUtility.BuildAnilistCharacterEmbed(character));
        }

        [Command("character")]
        [Alias("waifu", "char")]
        [Summary("Find a character from AniList GraphQL based on anilist character id.")]
        public async Task FindCharacterAsync([Remainder] int characterId)
        {
            CharacterResponse characterResponse = await _aniListFetcher.FindCharacterAsync(characterId);
            await ReplyAsync("", false, _embedUtility.BuildAnilistCharacterEmbed(characterResponse.Character));
        }

        [Command("character?")]
        [Alias("waifu?", "char?")]
        [Summary("Find a character from AniList GraphQL based on string criteria including spoilers.")]
        public async Task FindCharacterSpoilersAsync([Remainder] string searchCriteria)
        {
            PageResponse pageResponse = await _aniListFetcher.SearchCharactersAsync(searchCriteria);
            Character character = _levenshteinUtility.GetSingleBestCharacterResult(searchCriteria, pageResponse.Page.Characters);
            await ReplyAsync("", false, _embedUtility.BuildAnilistCharacterEmbed(character, true));
        }

        [Command("character?")]
        [Alias("waifu?", "char?")]
        [Summary("Find a character from AniList GraphQL based on anilist character id including spoilers.")]
        public async Task FindCharacterSpoilersAsync([Remainder] int characterId)
        {
            CharacterResponse characterResponse = await _aniListFetcher.FindCharacterAsync(characterId);
            await ReplyAsync("", false, _embedUtility.BuildAnilistCharacterEmbed(characterResponse.Character, true));
        }
    }
}
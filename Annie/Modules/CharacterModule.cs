using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.ResponseModels.Anilist;
using Discord.Interactions;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class CharacterModule : AbstractInteractionModule
    {
        public enum YesNo
        {
            Yes,
            No
        }

        /// <summary>
        /// Look for a spoiler-free Character entry from Anilist GraphQL database using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        [SlashCommand("character", "Find a character from AniList GraphQL based on string criteria or ID.")]
        public async Task FindCharacterAsync(
            [Summary(name: "search-criteria-or-id", description: "The search criteria to look for or the AniList ID of the character")] string args,
            [Summary(name: "allow-spoilers", description: "Allow spoilers in the character description?")] YesNo allowSpoilers = YesNo.No)
        {
            if (int.TryParse(args, out int characterId))
            { 
                CharacterResponse characterResponse = await _aniListFetcher.FindCharacterAsync(characterId);
                await FollowupAsync(isTTS: false, embed: _embedUtility.BuildAnilistCharacterEmbed(characterResponse.Character, allowSpoilers == YesNo.Yes), ephemeral: allowSpoilers == YesNo.Yes);
            } else {
                PageResponse pageResponse = await _aniListFetcher.SearchCharactersAsync(args);
                Character character = _levenshteinUtility.GetSingleBestCharacterResult(args, pageResponse.Page.Characters);
                await FollowupAsync(isTTS: false, embed: _embedUtility.BuildAnilistCharacterEmbed(character, allowSpoilers == YesNo.Yes), ephemeral: allowSpoilers == YesNo.Yes);
            }
        }
    }
}
using AnnieMayDiscordBot.ResponseModels.GitHub;
using AnnieMayDiscordBot.Services;
using Discord.Interactions;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    public class AboutModule : AbstractInteractionModule
    {
        protected GitHubFetcher _gitHubFetcher = new GitHubFetcher();

        /// <summary>
        /// Show behind-the-scenes information about Annie May.
        /// </summary>
        [SlashCommand("about", "Gives information regarding the Discord bot.")]
        public async Task AboutAsync()
        {
            // Fetch GitHub data.
            RepositoryResponse repositoryResponse = await _gitHubFetcher.FindGitHubRepository("Annie-May-Discord-Bot", "AlexanderColen");
            if (repositoryResponse == null || repositoryResponse.Repository == null)
            {
                await FollowupAsync(text: "The repository could not be found.");
                return;
            }
            await FollowupAsync(isTTS: false, embed: _embedUtility.BuildAboutEmbed(repositoryResponse.Repository));
        }
    }
}
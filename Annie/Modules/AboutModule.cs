using AnnieMayDiscordBot.ResponseModels.GitHub;
using AnnieMayDiscordBot.Services;
using Discord.Interactions;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("about", "Show information about this bot")]
    public class AboutModule : AbstractInteractionModule
    {
        protected GitHubFetcher _gitHubFetcher = new GitHubFetcher();

        /// <summary>
        /// Show behind-the-scenes information about Annie May.
        /// </summary>
        [SlashCommand("", "Gives information regarding the Discord bot.")]
        public async Task AboutAsync()
        {
            // Fetch GitHub data.
            RepositoryResponse repositoryResponse = await _gitHubFetcher.FindGitHubRepository("Annie-May-Discord-Bot", "AlexanderColen");
            if (repositoryResponse == null || repositoryResponse.Repository == null)
            {
                await RespondAsync(text: "The repository could not be found.");
                return;
            }
            await RespondAsync(isTTS: false, embed: _embedUtility.BuildAboutEmbed(repositoryResponse.Repository));
        }
    }
}
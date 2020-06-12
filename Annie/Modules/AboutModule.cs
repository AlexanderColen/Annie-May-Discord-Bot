using AnnieMayDiscordBot.ResponseModels.GitHub;
using AnnieMayDiscordBot.Services;
using Discord.Commands;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("about")]
    public class AboutModule : AbstractModule
    {
        protected GitHubFetcher _gitHubFetcher = new GitHubFetcher();

        [Command]
        [Summary("Gives information regarding the Discord bot.")]
        public async Task AboutAsync()
        {
            // Fetch GitHub data.
            RepositoryResponse repositoryResponse = await _gitHubFetcher.FindGitHubRepository("Annie-May-Discord-Bot", "AlexanderColen");
            if (repositoryResponse == null || repositoryResponse.Repository == null)
            {
                await ReplyAsync("The repository could not be found.");
                return;
            }
            await ReplyAsync("", false, _embedUtility.BuildAboutEmbed(repositoryResponse.Repository));
        }
    }
}
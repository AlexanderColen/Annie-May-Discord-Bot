﻿using AnnieMayDiscordBot.ResponseModels.GitHub;
using AnnieMayDiscordBot.Utility;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Services
{
    public class GitHubFetcher
    {
        private readonly GraphQLUtility _graphQLUtility = new GraphQLUtility("https://api.github.com/graphql", Properties.Resources.GITHUB_ACCESS_TOKEN);

        /// <summary>
        /// Find information about a GitHub repository.
        /// </summary>
        /// <param name="repositoryName">The name of the GitHub repository to find.</param>
        /// <param name="ownerName">The owner of this GitHub repository.</param>
        /// <param name="issues">The number of issues to show.</param>
        /// <param name="pullRequests">The number of pull requests to show.</param>
        /// <returns>The Repository response from GitHub v4 GraphQL API.</returns>
        public async Task<RepositoryResponse> FindGitHubRepository(string repositoryName, string ownerName, int issues = 5, int pullRequests = 5)
        {
            string query = @"
                query($name: String!, $owner: String!) {
                    repository(name: $name, owner: $owner) {
                        name
                        description
                        url
                        isPrivate
                        createdAt
                        pushedAt
                        primaryLanguage {
                            name
                        }
                    }
                }";

            object variables = new
            {
                name = repositoryName,
                owner = ownerName,
            };

            object headers = new
            {
                authorization = $"Bearer {Properties.Resources.GITHUB_ACCESS_TOKEN}"
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<RepositoryResponse>(query, variables);
        }
    }
}
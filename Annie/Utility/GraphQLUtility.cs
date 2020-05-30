using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace AnnieMayDiscordBot.Utility
{
    public class GraphQLUtility
    {
        private readonly IGraphQLClient _graphQLClient;
        
        public GraphQLUtility(string url)
        {
            _graphQLClient = new GraphQLHttpClient(url, new NewtonsoftJsonSerializer());
        }

        /// <summary>
        /// Execute a GraphQLRequest with a generic type response.
        /// </summary>
        /// <typeparam name="T">The class type of the response.</typeparam>
        /// <param name="query">The query for the GraphQLRequest.</param>
        /// <param name="variables">The variables for the GraphQLRequest.</param>
        /// <returns>The data outcome from the request as the set generic type.</returns>
        public async Task<T> ExecuteGraphQLRequest<T>(string query, object variables)
        {
            // Create request.
            var graphQLRequest = new GraphQLRequest
            {
                Query = query,
                Variables = variables
            };
            // Execute request.
            var graphQLResponse = await _graphQLClient.SendQueryAsync<T>(graphQLRequest);
            // Return Data from the request.
            return graphQLResponse.Data;
        }
    }
}

using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Utility
{
    public class GraphQLUtility
    {
        private readonly IGraphQLClient _graphQLClient;

        /// <summary>
        /// Constructor initializing the GraphQLClient based on the given URL.
        /// </summary>
        /// <param name="url">The URL of the GraphQL database.</param>
        /// <param name="token">Optional authorization bearer token.</param>
        public GraphQLUtility(string url, string token = null)
        {
            // If token is given, authenticate with it.
            if (!string.IsNullOrEmpty(token))
            {
                // Set endpoint.
                var options = new GraphQLHttpClientOptions
                {
                    EndPoint = new Uri(url)
                };
                // Set authorization header.
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _graphQLClient = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer(), httpClient);
            }
            // Otherwise try without.
            else
            {
                _graphQLClient = new GraphQLHttpClient(url, new NewtonsoftJsonSerializer());
            }
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
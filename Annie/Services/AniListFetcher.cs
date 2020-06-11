using AnnieMayDiscordBot.ResponseModels;
using AnnieMayDiscordBot.Utility;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Services
{
    public class AniListFetcher
    {
        private GraphQLUtility _graphQLUtility = new GraphQLUtility("https://graphql.anilist.co");

        /// <summary>
        /// Search for a list of Media from the Anilist GraphQL API using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        /// <param name="startPage">The result page number to start at.</param>
        /// <param name="entriesPerPage">The amount of entries every page should have.</param>
        /// <returns>The Page response from Anilist GraphQL API.</returns>
        public async Task<PageResponse> SearchMediaAsync(string searchCriteria, int startPage = 1, int entriesPerPage = 25)
        {
            string query = @"
            query ($page: Int, $perPage: Int, $search: String) {
                Page (page: $page, perPage: $perPage) {
                    pageInfo {
                        total
                        currentPage
                        lastPage
                        hasNextPage
                        perPage
                    }
                    media (search: $search) {
                        id
                        title {
                            english
                            romaji
                            native
                        }
                        type
                        status
                        description
                        season
                        seasonYear
                        episodes
                        duration
                        chapters
                        volumes
                        coverImage {
                            extraLarge
                        }
                        genres
                        synonyms
                        meanScore
                        popularity
                        favourites
                        siteUrl
                    }
                }
            }";

            object variables = new
            {
                search = searchCriteria,
                page = startPage,
                perPage = entriesPerPage
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<PageResponse>(query, variables);
        }

        /// <summary>
        /// Search for a list of Media of a specific MediaType from the Anilist GraphQL API using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        /// <param name="mediaType">The MediaType of the Media entry to look for. Either ANIME or MANGA.</param>
        /// <param name="startPage">The result page number to start at.</param>
        /// <param name="entriesPerPage">The amount of entries every page should have.</param>
        /// <returns>The Page response from Anilist GraphQL API.</returns>
        public async Task<PageResponse> SearchMediaTypeAsync(string searchCriteria, string mediaType, int startPage = 1, int entriesPerPage = 25)
        {
            string query = @"
                query ($page: Int, $perPage: Int, $search: String, $type: MediaType) {
                    Page (page: $page, perPage: $perPage) {
                        pageInfo {
                            total
                            currentPage
                            lastPage
                            hasNextPage
                            perPage
                        }
                        media (search: $search, type: $type) {
                            id
                            title {
                                english
                                romaji
                                native
                            }
                            type
                            status
                            description
                            season
                            seasonYear
                            episodes
                            duration
                            chapters
                            volumes
                            coverImage {
                                extraLarge
                            }
                            genres
                            synonyms
                            meanScore
                            popularity
                            favourites
                            siteUrl
                        }
                    }
                }";

            object variables = new
            {
                search = searchCriteria,
                type = mediaType.ToUpper(),
                page = startPage,
                perPage = entriesPerPage
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<PageResponse>(query, variables);
        }

        /// <summary>
        /// Search for a list of Characters from the Anilist GraphQL API using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        /// <param name="startPage">The result page number to start at.</param>
        /// <param name="entriesPerPage">The amount of entries every page should have.</param>
        /// <returns>The Page response from Anilist GraphQL API.</returns>
        public async Task<PageResponse> SearchCharactersAsync(string searchCriteria, int startPage = 1, int entriesPerPage = 25)
        {
            string query = @"
            query ($page: Int, $perPage: Int, $search: String) {
                Page (page: $page, perPage: $perPage) {
                    pageInfo {
                        total
                        currentPage
                        lastPage
                        hasNextPage
                        perPage
                    }
                    characters(search: $search) {
                        id
                        name {
                            full
                            native
                            alternative
                        }
                        image {
                            large
                        }
                        description
                        siteUrl
                        media {
                            nodes {
                                id
                                title {
                                    english
                                    romaji
                                    native
                                }
                                type
                                siteUrl
                            }
                            edges {
                                characterRole
                            }
                        }
                        favourites
                    }
                }
            }";

            object variables = new
            {
                search = searchCriteria,
                page = startPage,
                perPage = entriesPerPage
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<PageResponse>(query, variables);
        }

        /// <summary>
        /// Search for a list of Staff from the Anilist GraphQL API using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        /// <param name="startPage">The result page number to start at.</param>
        /// <param name="entriesPerPage">The amount of entries every page should have.</param>
        /// <returns>The Page response from Anilist GraphQL API.</returns>
        public async Task<PageResponse> SearchStaffAsync(string searchCriteria, int startPage = 1, int entriesPerPage = 25)
        {
            string query = @"
            query ($page: Int, $perPage: Int, $search: String) {
                Page (page: $page, perPage: $perPage) {
                    pageInfo {
                        total
                        currentPage
                        lastPage
                        hasNextPage
                        perPage
                    }
                    staff(search: $search) {
                        id
                        name {
                            full
                            native
                            alternative
                        }
                        language
                        image {
                            large
                        }
                        description
                        siteUrl
                        staffMedia {
                            nodes {
                                title {
                                    english
                                    romaji
                                }
                                siteUrl
                            }
                            edges {
                                staffRole
                            }
                        }
                        characters {
                            nodes {
                                name {
                                    full
                                    native
                                }
                                siteUrl
                            }
                            edges {
                                role
                            }
                        }
                        favourites
                    }
                }
            }";

            object variables = new
            {
                search = searchCriteria,
                page = startPage,
                perPage = entriesPerPage
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<PageResponse>(query, variables);
        }

        /// <summary>
        /// Search for a list of Studios from the Anilist GraphQL API using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        /// <param name="startPage">The result page number to start at.</param>
        /// <param name="entriesPerPage">The amount of entries every page should have.</param>
        /// <returns>The Page response from Anilist GraphQL API.</returns>
        public async Task<PageResponse> SearchStudiosAsync(string searchCriteria, int startPage = 1, int entriesPerPage = 25)
        {
            string query = @"
            query ($page: Int, $perPage: Int, $search: String) {
                Page (page: $page, perPage: $perPage) {
                    pageInfo {
                        total
                        currentPage
                        lastPage
                        hasNextPage
                        perPage
                    }
                    studios(search: $search) {
                        id
                        name
                        isAnimationStudio
                        media {
                            nodes {
                                id
                                title {
                                    english
                                    romaji
                                    native
                                }
                                type
                                siteUrl
                            }
                            edges {
                                isMainStudio
                            }
                        }
                        siteUrl
                        favourites
                    }
                }
            }";

            object variables = new
            {
                search = searchCriteria,
                page = startPage,
                perPage = entriesPerPage
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<PageResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Media from the Anilist GraphQL API using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        /// <returns>The Media response from Anilist GraphQL API.</returns>
        public async Task<MediaResponse> FindMediaAsync(string searchCriteria)
        {
            string query = @"
                query ($search: String) {
                    Media (search: $search) {
                        id
                        idMal
                        title {
                            english
                            romaji
                            native
                        }
                        type
                        status
                        description
                        season
                        seasonYear
                        episodes
                        duration
                        chapters
                        volumes
                        coverImage {
                            extraLarge
                        }
                        genres
                        synonyms
                        meanScore
                        popularity
                        favourites
                        siteUrl
                    }
                }";

            object variables = new
            {
                search = searchCriteria
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<MediaResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Media from the Anilist GraphQL API using the Anilist Media ID.
        /// </summary>
        /// <param name="mediaId">The Anilist ID of the Media entry.</param>
        /// <returns>The Media response from Anilist GraphQL API.</returns>
        public async Task<MediaResponse> FindMediaAsync(int mediaId)
        {
            string query = @"
                query ($id: Int) {
                    Media (id: $id) {
                        id
                        idMal
                        title {
                            english
                            romaji
                            native
                        }
                        type
                        status
                        description
                        season
                        seasonYear
                        episodes
                        duration
                        chapters
                        volumes
                        coverImage {
                            extraLarge
                        }
                        genres
                        synonyms
                        meanScore
                        popularity
                        favourites
                        siteUrl
                    }
                }";

            object variables = new
            {
                id = mediaId
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<MediaResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Media of a specific MediaType from the Anilist GraphQL API using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        /// <param name="mediaType">The MediaType of the Media entry to look for. Either ANIME or MANGA.</param>
        /// <returns>The Media response from Anilist GraphQL API.</returns>
        public async Task<MediaResponse> FindMediaTypeAsync(string searchCriteria, string mediaType)
        {
            string query = @"
                query ($search: String, $type: MediaType) {
                    Media (search: $search, type: $type) {
                        id
                        idMal
                        title {
                            english
                            romaji
                            native
                        }
                        type
                        status
                        description
                        season
                        seasonYear
                        episodes
                        duration
                        chapters
                        volumes
                        coverImage {
                            extraLarge
                        }
                        genres
                        synonyms
                        meanScore
                        popularity
                        favourites
                        siteUrl
                    }
                }";

            object variables = new
            {
                search = searchCriteria,
                type = mediaType.ToUpper()
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<MediaResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Media of a specific MediaType from the Anilist GraphQL API using Anilist Media ID.
        /// </summary>
        /// <param name="mediaId">The Anilist ID of the Media entry.</param>
        /// <param name="mediaType">The MediaType of the Media entry. Either ANIME or MANGA.</param>
        /// <returns>The Media response from Anilist GraphQL API.</returns>
        public async Task<MediaResponse> FindMediaTypeAsync(int mediaId, string mediaType)
        {
            string query = @"
                query ($id: Int, $type: MediaType) {
                    Media (id: $id, type: $type) {
                        id
                        idMal
                        title {
                            english
                            romaji
                            native
                        }
                        type
                        status
                        description
                        season
                        seasonYear
                        episodes
                        duration
                        chapters
                        volumes
                        coverImage {
                            extraLarge
                        }
                        genres
                        synonyms
                        meanScore
                        popularity
                        favourites
                        siteUrl
                    }
                }";

            object variables = new
            {
                id = mediaId,
                type = mediaType.ToUpper()
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<MediaResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Character from the Anilist GraphQL API using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        /// <returns>The Character response from Anilist GraphQL API.</returns>
        public async Task<CharacterResponse> FindCharacterAsync(string searchCriteria)
        {
            string query = @"
                query ($search: String) {
                    Character(search: $search) {
                        id
                        name {
                            full
                            native
                            alternative
                        }
                        image {
                            large
                        }
                        description
                        siteUrl
                        media {
                            nodes {
                                id
                                title {
                                    english
                                    romaji
                                    native
                                }
                                type
                                siteUrl
                            }
                            edges {
                                characterRole
                            }
                        }
                        favourites
                    }
                }";

            object variables = new
            {
                search = searchCriteria,
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<CharacterResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Character from the Anilist GraphQL API using Anilist Character ID.
        /// </summary>
        /// <param name="characterId">The Anilist ID of the Character entry.</param>
        /// <returns>The Character response from Anilist GraphQL API.</returns>
        public async Task<CharacterResponse> FindCharacterAsync(int characterId)
        {
            string query = @"
                query ($id: Int) {
                    Character(id: $id) {
                        id
                        name {
                            full
                            native
                            alternative
                        }
                        image {
                            large
                        }
                        description
                        siteUrl
                        media {
                            nodes {
                                id
                                title {
                                    english
                                    romaji
                                    native
                                }
                                type
                                siteUrl
                            }
                            edges {
                                characterRole
                            }
                        }
                        favourites
                    }
                }";

            object variables = new
            {
                id = characterId,
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<CharacterResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Staff from the Anilist GraphQL API using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        /// <returns>The Staff response from Anilist GraphQL API.</returns>
        public async Task<StaffResponse> FindStaffAsync(string searchCriteria)
        {
            string query = @"
                query ($search: String) {
                    Staff(search: $search) {
                        id
                        name {
                            full
                            native
                            alternative
                        }
                        language
                        image {
                            large
                        }
                        description
                        siteUrl
                        staffMedia {
                            nodes {
                                title {
                                    english
                                    romaji
                                }
                                siteUrl
                            }
                            edges {
                                staffRole
                            }
                        }
                        characters {
                            nodes {
                                name {
                                    full
                                    native
                                }
                                siteUrl
                            }
                            edges {
                                role
                            }
                        }
                        favourites
                    }
                }";

            object variables = new
            {
                search = searchCriteria,
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<StaffResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Staff from the Anilist GraphQL API using Anilist Studio ID.
        /// </summary>
        /// <param name="staffId">The Anilist ID of the Studio entry.</param>
        /// <returns>The Staff response from Anilist GraphQL API.</returns>
        public async Task<StaffResponse> FindStaffAsync(int staffId)
        {
            string query = @"
                query ($id: Int) {
                    Staff(id: $id) {
                        id
                        name {
                            full
                            native
                            alternative
                        }
                        language
                        image {
                            large
                        }
                        description
                        siteUrl
                        staffMedia {
                            nodes {
                                title {
                                    english
                                    romaji
                                }
                                siteUrl
                            }
                            edges {
                                staffRole
                            }
                        }
                        characters {
                            nodes {
                                name {
                                    full
                                    native
                                }
                                siteUrl
                            }
                            edges {
                                role
                            }
                        }
                        favourites
                    }
                }";

            object variables = new
            {
                id = staffId,
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<StaffResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Studio from the Anilist GraphQL API using search criteria.
        /// </summary>
        /// <param name="searchCriteria">The criteria to search for.</param>
        /// <returns>The Studio response from Anilist GraphQL API.</returns>
        public async Task<StudioResponse> FindStudioAsync(string searchCriteria)
        {
            string query = @"
                query ($search: String) {
	                Studio(search: $search) {
                        id
                        name
                        isAnimationStudio
                        media {
                            nodes {
                                id
                                title {
                                    english
                                    romaji
                                    native
                                }
                                type
                                siteUrl
                            }
                            edges {
                                isMainStudio
                            }
                        }
                        siteUrl
                        favourites
                    }
                }";

            object variables = new
            {
                search = searchCriteria,
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<StudioResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Studio from the Anilist GraphQL API using Anilist Studio ID.
        /// </summary>
        /// <param name="studioId">The Anilist ID of the Studio entry.</param>
        /// <returns>The Studio response from Anilist GraphQL API.</returns>
        public async Task<StudioResponse> FindStudioAsync(int studioId)
        {
            string query = @"
                query ($id: Int) {
	                Studio(id: $id) {
                        id
                        name
                        isAnimationStudio
                        media {
                            nodes {
                                id
                                title {
                                    english
                                    romaji
                                    native
                                }
                                type
                                siteUrl
                            }
                            edges {
                                isMainStudio
                            }
                        }
                        siteUrl
                        favourites
                    }
                }";

            object variables = new
            {
                id = studioId,
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<StudioResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Anilist User from the Anilist GraphQL API using their username.
        /// </summary>
        /// <param name="anilistName">The username of the Anilist user.</param>
        /// <returns>The User response from Anilist GraphQL API.</returns>
        public async Task<UserResponse> FindUserAsync(string anilistName)
        {
            string query = @"
                query ($name: String) {
                    User(name: $name) {
                        id
                        name
                    }
                }";

            object variables = new
            {
                name = anilistName
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<UserResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Anilist User from the Anilist GraphQL API using their Anilist User ID.
        /// </summary>
        /// <param name="anilistId">The ID of the Anilist user.</param>
        /// <returns>The User response from Anilist GraphQL API.</returns>
        public async Task<UserResponse> FindUserAsync(long anilistId)
        {
            string query = @"
                query ($id: Int) {
                    User(id: $id) {
                        id
                        name
                    }
                }";

            object variables = new
            {
                id = anilistId
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<UserResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Anilist User from the Anilist GraphQL API using their username.
        /// </summary>
        /// <param name="anilistName">The username of the Anilist user.</param>
        /// <returns>The User response from Anilist GraphQL API with all their list data.</returns>
        public async Task<UserResponse> FindUserStatisticsAsync(string anilistName)
        {
            string query = @"
                query ($name: String) {
                    User(name: $name) {
                        name
                        about
                        avatar {
                            large
                            medium
                        }
                        bannerImage
                        statistics {
                            anime {
                                count
                                meanScore
                                standardDeviation
                                minutesWatched
                                episodesWatched
                                chaptersRead
                                volumesRead
                            }
                            manga {
                                count
                                meanScore
                                standardDeviation
                                minutesWatched
                                episodesWatched
                                chaptersRead
                                volumesRead
                            }
                        }
                        siteUrl
                        updatedAt
                    }
                }";

            object variables = new
            {
                name = anilistName
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<UserResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Anilist User from the Anilist GraphQL API using their Anilist User ID.
        /// </summary>
        /// <param name="anilistId">The ID of the Anilist user.</param>
        /// <returns>The User response from Anilist GraphQL API with all their list data.</returns>
        public async Task<UserResponse> FindUserStatisticsAsync(long anilistId)
        {
            string query = @"
                query ($id: Int) {
                  User(id: $id) {
                    id
                    name
                    about
                    avatar {
                      large
                      medium
                    }
                    bannerImage
                    statistics {
                      anime {
                        count
                        meanScore
                        standardDeviation
                        minutesWatched
                        episodesWatched
                        chaptersRead
                        volumesRead
                      }
                      manga {
                        count
                        meanScore
                        standardDeviation
                        minutesWatched
                        episodesWatched
                        chaptersRead
                        volumesRead
                      }
                    }
                    siteUrl
                    updatedAt
                  }
                }";

            object variables = new
            {
                id = anilistId
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<UserResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Anilist User's Media list for a specific MediaType using their username.
        /// </summary>
        /// <param name="anilistName">The username of the Anilist user.</param>
        /// <param name="mediaType">The MediaType of the list. Either ANIME or MANGA.</param>
        /// <returns>The MediaCollection response from Anilist GraphQL API.</returns>
        public async Task<MediaListCollectionResponse> FindUserListAsync(string anilistName, string mediaType)
        {
            string query = @"
                query ($userName: String, $type: MediaType) {
                    MediaListCollection(userName: $userName, type: $type) {
                        lists {
                            entries {
                                mediaId
                                status
                                score(format:POINT_100)
                                progress
                                media {
                                    id
                                    title {
                                        english
                                        romaji
                                        native
                                    }
                                    type
                                    status
                                }
                            }
                            name
                            status
                        }
                    }
                }";

            object variables = new
            {
                userName = anilistName,
                type = mediaType.ToUpper()
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<MediaListCollectionResponse>(query, variables);
        }

        /// <summary>
        /// Find a specific Anilist User's Media list for a specific MediaType using their Anilist User ID.
        /// </summary>
        /// <param name="anilistId">The ID of the Anilist user.</param>
        /// <param name="mediaType">The MediaType of the list. Either ANIME or MANGA.</param>
        /// <returns>The MediaCollection response from Anilist GraphQL API.</returns>
        public async Task<MediaListCollectionResponse> FindUserListAsync(long anilistId, string mediaType)
        {
            string query = @"
                query ($id: Int, $type: MediaType) {
                    MediaListCollection(userId: $id, type: $type) {
                        lists {
                            entries {
                                mediaId
                                status
                                score(format:POINT_100)
                                progress
                                media {
                                    id
                                    title {
                                        english
                                        romaji
                                        native
                                    }
                                    type
                                    status
                                }
                            }
                            name
                            status
                        }
                    }
                }";

            object variables = new
            {
                id = anilistId,
                type = mediaType.ToUpper()
            };

            return await _graphQLUtility.ExecuteGraphQLRequest<MediaListCollectionResponse>(query, variables);
        }
    }
}
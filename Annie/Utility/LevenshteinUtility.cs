using AnnieMayDiscordBot.Models.Anilist;
using F23.StringSimilarity;
using System;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Utility
{
    public class LevenshteinUtility
    {
        private NormalizedLevenshtein normalizedLevenshtein = new NormalizedLevenshtein();

        /// <summary>
        /// Calculate the best corresponding Media from a list of Media based on the searched string.
        /// </summary>
        /// <param name="searchQuery">The string to search for.</param>
        /// <param name="mediaList">The list of Media objects that need to be searched through.</param>
        /// <returns>The Media that was the closest distance to the search query.</returns>
        public Media GetSingleBestResult(string searchQuery, List<Media> mediaList)
        {
            double bestResult = 0.0;
            Media mediaResult = null;
            foreach (Media media in mediaList)
            {
                List<string> possibleTitles = media.synonyms;
                possibleTitles.Add(media.title.english);
                possibleTitles.Add(media.title.native);
                possibleTitles.Add(media.title.romaji);

                foreach (string possibleTitle in possibleTitles)
                {
                    if (searchQuery != null && possibleTitle != null)
                    {
                        double current = normalizedLevenshtein.Distance(searchQuery, possibleTitle);

                        if (current > bestResult)
                        {
                            bestResult = current;
                            mediaResult = media;
                        }
                    }
                }
            }

            return mediaResult;
        }

        /// <summary>
        /// Calculates the distance between two strings using the Levenshtein algorithm.
        /// </summary>
        /// <param name="a">The first string to compare to.</param>
        /// <param name="b">The second string to compare to.</param>
        /// <returns>The distance between the two strings.</returns>
        private int CalculateDistance(string a, string b)
        {
            // https://stackoverflow.com/a/9453762
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                return 0;
            }

            if (string.IsNullOrEmpty(a))
            {
                return b.Length;
            }

            if (string.IsNullOrEmpty(b))
            {
                return a.Length;
            }

            a = a.ToLower();
            b = b.ToLower();

            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                    (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                    );
                }

            return distances[lengthA, lengthB];
        }
    }
}
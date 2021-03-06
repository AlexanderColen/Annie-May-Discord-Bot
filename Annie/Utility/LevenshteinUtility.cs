﻿using AnnieMayDiscordBot.Models.Anilist;
using F23.StringSimilarity;
using System;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Utility
{
    public class LevenshteinUtility
    {
        private readonly NormalizedLevenshtein normalizedLevenshtein = new NormalizedLevenshtein();

        /// <summary>
        /// Calculate the best corresponding Media from a list of Media based on the searched string.
        /// </summary>
        /// <param name="searchQuery">The string to search for.</param>
        /// <param name="mediaList">The list of Media objects that need to be searched through.</param>
        /// <returns>The Media that was the closest distance to the search query.</returns>
        public Media GetSingleBestMediaResult(string searchQuery, List<Media> mediaList)
        {
            double bestResult = 0.0;
            Media mediaResult = null;
            foreach (Media media in mediaList)
            {
                List<string> possibleTitles = media.Synonyms;
                possibleTitles.Add(media.Title.English);
                possibleTitles.Add(media.Title.Native);
                possibleTitles.Add(media.Title.Romaji);

                foreach (string possibleTitle in possibleTitles)
                {
                    if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(possibleTitle))
                    {
                        // If names are the same when lowercased, instantly return this entry.
                        if (searchQuery.ToLower().Equals(possibleTitle.ToLower()))
                        {
                            return media;
                        }

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
        /// Calculate the best corresponding Character from a list of Characters based on the searched string.
        /// </summary>
        /// <param name="searchQuery">The string to search for.</param>
        /// <param name="characterList">The list of Character objects that need to be searched through.</param>
        /// <returns>The Character that was the closest distance to the search query.</returns>
        public Character GetSingleBestCharacterResult(string searchQuery, List<Character> characterList)
        {
            double bestResult = 0.0;
            Character characterResult = null;
            foreach (Character character in characterList)
            {
                List<string> possibleNames = character.Name.Alternative;
                possibleNames.Add(character.Name.Full);
                possibleNames.Add(character.Name.Native);

                foreach (string possibleName in possibleNames)
                {
                    if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(possibleName))
                    {
                        // If names are the same when lowercased, instantly return this entry.
                        if (searchQuery.ToLower().Equals(possibleName.ToLower()))
                        {
                            return character;
                        }

                        double current = normalizedLevenshtein.Distance(searchQuery, possibleName);

                        if (current > bestResult)
                        {
                            bestResult = current;
                            characterResult = character;
                        }
                    }
                }
            }

            return characterResult;
        }

        /// <summary>
        /// Calculate the best corresponding Staff from a list of Staff based on the searched string.
        /// </summary>
        /// <param name="searchQuery">The string to search for.</param>
        /// <param name="staffList">The list of Staff objects that need to be searched through.</param>
        /// <returns>The Staff that was the closest distance to the search query.</returns>
        public Staff GetSingleBestStaffResult(string searchQuery, List<Staff> staffList)
        {
            double bestResult = 0.0;
            Staff staffResult = null;
            foreach (Staff staff in staffList)
            {
                List<string> possibleNames = staff.Name.Alternative;
                possibleNames.Add(staff.Name.Full);
                possibleNames.Add(staff.Name.Native);

                foreach (string possibleName in possibleNames)
                {
                    if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(possibleName))
                    {
                        // If names are the same when lowercased, instantly return this entry.
                        if (searchQuery.ToLower().Equals(possibleName.ToLower()))
                        {
                            return staff;
                        }

                        double current = normalizedLevenshtein.Distance(searchQuery, possibleName);

                        if (current > bestResult)
                        {
                            bestResult = current;
                            staffResult = staff;
                        }
                    }
                }
            }

            return staffResult;
        }

        /// <summary>
        /// Calculate the best corresponding Studio from a list of Studios based on the searched string.
        /// </summary>
        /// <param name="searchQuery">The string to search for.</param>
        /// <param name="studioList">The list of Studio objects that need to be searched through.</param>
        /// <returns>The Studio that was the closest distance to the search query.</returns>
        public Studio GetSingleBestStudioResult(string searchQuery, List<Studio> studioList)
        {
            double bestResult = 0.0;
            Studio studioResult = null;
            foreach (Studio studio in studioList)
            {
                if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(studio.Name))
                {
                    // If names are the same when lowercased, instantly return this entry.
                    if (searchQuery.ToLower().Equals(studio.Name.ToLower()))
                    {
                        return studio;
                    }

                    double current = normalizedLevenshtein.Distance(searchQuery, studio.Name);

                    if (current > bestResult)
                    {
                        bestResult = current;
                        studioResult = studio;
                    }
                }
            }

            return studioResult;
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
            {
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                    (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                    );
                }
            }

            return distances[lengthA, lengthB];
        }
    }
}
using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Models.Anilist;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnnieMayDiscordBot.Utility
{
    // Singleton class.
    public class AffinityUtility
    {
        private static readonly AffinityUtility _affinityUtility = new AffinityUtility();

        private AffinityUtility() { }

        public static AffinityUtility GetInstance() => _affinityUtility;

        /// <summary>
        /// Fetch the shared media between two user's Media lists.
        /// </summary>
        /// <param name="mediaListA">The Media list of user A.</param>
        /// <param name="mediaListB">The Media list of user B.</param>
        /// <returns>The list of shared Media entries.</returns>
        public List<SharedMedia> GetSharedMedia(List<MediaList> mediaListA, List<MediaList> mediaListB)
        {
            // No affinity if either list is null, thus it should be an empty return.
            if (mediaListA == null || mediaListB == null)
            {
                return new List<SharedMedia>();
            }

            // Find non-duplicate shared Media entries and save their scores.
            var sharedMedia = new List<SharedMedia>();
            var uniqueIDs = new List<int>();
            foreach (var mediaA in mediaListA)
            {
                foreach (var mediaB in mediaListB)
                {
                    // Shared media entry.
                    if (mediaA.MediaId.Equals(mediaB.MediaId)
                        // Make sure it's not a duplicate entry that is in the list multiple times.
                        && !uniqueIDs.Contains(mediaA.MediaId)
                        /*
                         * Score needs to not be 0 for either to filter out Planned/unscored entries
                         * that will ruin affinity calculations otherwise.
                         */
                        && mediaA.Score != 0 && mediaB.Score != 0)
                    {
                        sharedMedia.Add(new SharedMedia
                        {
                            MediaId = mediaA.MediaId,
                            ScoreA = mediaA.Score,
                            ScoreB = mediaB.Score
                        });
                        uniqueIDs.Add(mediaA.MediaId);
                        break;
                    }
                }
            }

            return sharedMedia;
        }

        /// <summary>
        /// Calculate the Pearson correlation coefficient with the list of SharedMedia.
        /// </summary>
        /// <param name="sharedMedia">The list of SharedMedia with scores.</param>
        /// <returns>A double with the Pearson correlation coefficient.</returns>
        public double CalculatePearsonAffinity(List<SharedMedia> sharedMedia)
        {
            // Get all scores in separate lists.
            var scoresA = sharedMedia.Select(x => x.ScoreA).ToArray();
            var scoresB = sharedMedia.Select(x => x.ScoreB).ToArray();
            
            /*
             * Calculate Pearson correlation coefficient.
             * Based on https://en.wikipedia.org/wiki/Pearson_correlation_coefficient
             * and https://github.com/jerome-ceccato/andre/blob/1ac2500605c18e9da6cc044793af157d8d40fc73/commands/mal.py#L695-L718
             */
            var ma = scoresA.Sum() / scoresA.Length;
            var mb = scoresB.Sum() / scoresB.Length;
            
            var am = scoresA.Select(x => x - ma);
            var bm = scoresB.Select(x => x - ma);
            
            var sa = am.Select(x => Math.Pow(x, 2));
            var sb = bm.Select(x => Math.Pow(x, 2));

            var numerator = am.Zip(bm).Select(x => x.First * x.Second).Sum();
            var denominator = Math.Sqrt(sa.Sum() * sb.Sum());

            // Cannot divide by zero.
            if (denominator == 0.0)
            {
                throw new DivideByZeroException();
            }
            
            return numerator / denominator;
        }
    }
}
using AnnieMayDiscordBot.Models.Anilist;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        /// Calculate the Pearson affinity between two user's Media lists.
        /// </summary>
        /// <param name="mediaListA">The MediaList of user A.</param>
        /// <param name="mediaListB">The MediaList of user B.</param>
        /// <returns>A float indicating the Pearson affinity between two users Media lists.</returns>
        public List<MediaList> GetSharedMedia(List<MediaList> mediaListA, List<MediaList> mediaListB)
        {
            // No affinity if either list is empty/null.
            if (mediaListA == null || mediaListB == null || mediaListA.Count == 0 || mediaListB.Count == 0)
            {
                return null;
            }

            // Shared media.
            var sharedMedia = mediaListA.Intersect(mediaListB, new MediaListComparer());

            return sharedMedia.ToList();
        }

        // Comparer class to check if MediaList entries that are scored are on both lists.
        internal class MediaListComparer : IEqualityComparer<MediaList>
        {
            public bool Equals(MediaList x, MediaList y)
            {
                if (x.Score == 0 || y.Score == 0)
                {
                    return false;
                }

                return x.MediaId == y.MediaId;
            }

            public int GetHashCode([DisallowNull] MediaList obj)
            {
                return obj.MediaId.GetHashCode();
            }
        }
    }
}
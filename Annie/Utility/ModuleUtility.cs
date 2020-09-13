using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Utility
{
    // Singleton class.
    public class ModuleUtility
    {
        private static readonly ModuleUtility _moduleUtilityInstance = new ModuleUtility();
        
        private ModuleUtility() { }

        public static ModuleUtility GetInstance() => _moduleUtilityInstance;

        /// <summary>
        /// Try to fetch a user's Anilist ID based on the given userId.
        /// </summary>
        /// <param name="userId">The ID</param>
        /// <returns></returns>
        public async Task<long?> GetAnilistIDAsync(long userId)
        {
            // Check if long is not a Discord User ID (17-18 digits).
            if (userId.ToString().Length < 17)
            {
                // It most likely was already an Anilist ID in this case.
                return userId;
            }
            
            var user = await DatabaseUtility.GetInstance().GetSpecificUserAsync((ulong)userId);
            if (user == null)
            {
                // No found Anilist ID for the user.
                return null;
            }

            // User with Anilist ID found.
            return user.AnilistId;
        }
    }
}
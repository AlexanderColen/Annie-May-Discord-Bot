using AnnieMayDiscordBot.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Utility
{
    // Singleton class.
    public class DatabaseUtility
    {
        private static readonly DatabaseUtility _dbUtilityInstance = new DatabaseUtility();
        
        private DatabaseUtility() { }

        public static DatabaseUtility GetInstance() => _dbUtilityInstance;

        /// <summary>
        /// Get a list of all the Users from the database.
        /// </summary>
        /// <returns>A list of all the DiscordUsers.</returns>
        public async Task<List<DiscordUser>> GetUsersAsync()
        {
            await using var conn = new NpgsqlConnection(Properties.Resources.DATABASE_URI);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand("SELECT * FROM annie_may.user", conn);
            await cmd.PrepareAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            var users = new List<DiscordUser>();

            while (await reader.ReadAsync())
            {
                var user = new DiscordUser()
                {
                    Id = reader.GetInt32(0),
                    DiscordId = (ulong)reader.GetInt64(1),
                    Name = reader.GetString(2),
                    AnilistId = reader.GetInt32(3),
                    AnilistName = reader.GetString(4)
                };
                users.Add(user);
            }

            return users;
        }

        /// <summary>
        /// Get a specific User from the database.
        /// </summary>
        /// <param name="discordID">The ID of a Discord user.</param>
        /// <returns>The first DiscordUser found, otherwise null.</returns>
        public async Task<DiscordUser> GetSpecificUserAsync(ulong discordID)
        {
            await using var conn = new NpgsqlConnection(Properties.Resources.DATABASE_URI);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand("SELECT * FROM annie_may.user WHERE discordid = @discordId", conn))
            {
                cmd.Parameters.AddWithValue("discordId", (decimal) discordID);
                await cmd.PrepareAsync();
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var user = new DiscordUser()
                    {
                        Id = reader.GetInt32(0),
                        DiscordId = (ulong) reader.GetDecimal(1),
                        Name = reader.GetString(2),
                        AnilistId = reader.GetInt32(3),
                        AnilistName = reader.GetString(4)
                    };
                    return user;
                }
            };

            return null;
        }

        /// <summary>
        /// Get a specific Guild's settings from the database.
        /// </summary>
        /// <param name="guildID">The ID of a Discord guild.</param>
        /// <returns>The first GuildSettings found, otherwise null.</returns>
        public async Task<GuildSettings> GetSpecificGuildSettingsAsync(ulong guildID)
        {
            await using var conn = new NpgsqlConnection(Properties.Resources.DATABASE_URI);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand("SELECT * FROM annie_may.guild_settings WHERE guildId = @guildID", conn))
            {
                cmd.Parameters.AddWithValue("guildID", (decimal)guildID);
                await cmd.PrepareAsync();
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var guildSettings = new GuildSettings()
                    {
                        Id = reader.GetInt32(0),
                        GuildId = (ulong)reader.GetDecimal(1),
                        Prefix = reader.GetString(2),
                        ShowUserScores = reader.GetBoolean(3)
                    };
                    return guildSettings;
                }
            };

            return null;
        }

        /// <summary>
        /// Get a list of the custom status messages from the database.
        /// </summary>
        /// <returns>A List with all the statuses from the database. Can be empty.</returns>
        public async Task<List<string>> GetCustomStatuses()
        {
            await using var conn = new NpgsqlConnection(Properties.Resources.DATABASE_URI);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand("SELECT statustext FROM annie_may.status", conn);
            await cmd.PrepareAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            var statuses = new List<string>();
            while (await reader.ReadAsync())
            {
                statuses.Add(reader.GetString(0));
            }
            return statuses;
        }

        /// <summary>
        /// Insert or update a User in the database.
        /// </summary>
        /// <param name="discordUser">The DiscordUser to insert or update.</param>
        /// <returns>True if the action was successful, false otherwise.</returns>
        public async Task<bool> UpsertUserAsync(DiscordUser discordUser)
        {
            await using var conn = new NpgsqlConnection(Properties.Resources.DATABASE_URI);
            await conn.OpenAsync();
            // Check if Discord ID has not been used yet, if so register a new user.
            if (await GetSpecificUserAsync(discordUser.DiscordId) == null)
            {
                await using var cmd = new NpgsqlCommand("INSERT INTO annie_may.user (DiscordId, Name, AnilistId, AnilistName) " +
                                                        "VALUES (@discordId, @name, @anilistId, @anilistName);", conn);
                cmd.Parameters.AddWithValue("discordId", (decimal) discordUser.DiscordId);
                cmd.Parameters.AddWithValue("name", discordUser.Name);
                cmd.Parameters.AddWithValue("anilistId", discordUser.AnilistId);
                cmd.Parameters.AddWithValue("anilistName", discordUser.AnilistName);
                await cmd.PrepareAsync();
                return await cmd.ExecuteNonQueryAsync() == 1;
            } else
            {
                await using var cmd = new NpgsqlCommand("UPDATE annie_may.user " +
                                                        "SET anilistid = @anilistId, anilistname = @anilistName " +
                                                        "WHERE discordid = @discordId;", conn);
                cmd.Parameters.AddWithValue("discordId", (decimal) discordUser.DiscordId);
                cmd.Parameters.AddWithValue("anilistId", discordUser.AnilistId);
                cmd.Parameters.AddWithValue("anilistName", discordUser.AnilistName);
                await cmd.PrepareAsync();
                return await cmd.ExecuteNonQueryAsync() == 1;
            }
        }

        /// <summary>
        /// Insert of update the settings for a Guild.
        /// </summary>
        /// <param name="guildID">The ID of the Guild.</param>
        /// <returns>True if it was succesful, false otherwise.</returns>
        public async Task<bool> UpsertGuildSettings(GuildSettings guildSettings)
        {
            await using var conn = new NpgsqlConnection(Properties.Resources.DATABASE_URI);
            await conn.OpenAsync();
            // Check if Guild ID has not been used yet, if so register a new Guild.
            if (await GetSpecificGuildSettingsAsync(guildSettings.GuildId) == null)
            {
                await using var cmd = new NpgsqlCommand("INSERT INTO annie_may.guild_settings (GuildId, Prefix, ShowUserScores) " +
                                                        "VALUES (@guildId, @prefix, @userScores);", conn);
                cmd.Parameters.AddWithValue("guildId", (decimal)guildSettings.GuildId);
                cmd.Parameters.AddWithValue("prefix", guildSettings.Prefix);
                cmd.Parameters.AddWithValue("userScores", guildSettings.ShowUserScores);
                await cmd.PrepareAsync();

                if (await cmd.ExecuteNonQueryAsync() == 1)
                {
                    CacheUtility.GetInstance().CachedGuildSettings.Add(guildSettings.GuildId, guildSettings);
                    return true;
                }
                return false;
            }
            else
            {
                await using var cmd = new NpgsqlCommand("UPDATE annie_may.guild_settings " +
                                                        "SET prefix = @prefix, showuserscores = @userScores " +
                                                        "WHERE guildId = @guildId;", conn);
                cmd.Parameters.AddWithValue("guildId", (decimal)guildSettings.GuildId);
                cmd.Parameters.AddWithValue("prefix", guildSettings.Prefix);
                cmd.Parameters.AddWithValue("userScores", guildSettings.ShowUserScores);
                await cmd.PrepareAsync();

                if (await cmd.ExecuteNonQueryAsync() == 1)
                {
                    CacheUtility.GetInstance().CachedGuildSettings.Add(guildSettings.GuildId, guildSettings);
                    return true;
                }
                return false;
            }
        }
    }
}
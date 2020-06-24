using AnnieMayDiscordBot.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Utility
{
    public class DatabaseUtility
    {
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
    }
}
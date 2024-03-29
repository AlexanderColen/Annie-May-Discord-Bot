﻿using AnnieMayDiscordBot.Enums.Anilist;
using AnnieMayDiscordBot.Models.Anilist;
using AnnieMayDiscordBot.Properties;
using AnnieMayDiscordBot.Utility;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot.Modules
{
    [Group("random", "Try your luck with various random generation")]
    public class RandomModule : AbstractInteractionModule
    {
        /// <summary>
        /// Roll a die.
        /// </summary>
        [SlashCommand("roll", "Roll a die to get a random number.")]
        public async Task RollAsync()
        {
            Random random = new Random((int)DateTime.UtcNow.Ticks);
            await FollowupAsync(text: $"神様 has blessed you with a {random.Next()}.", isTTS: false);
        }

        /// <summary>
        /// Roll a die up to a custom maximum.
        /// <param name="max">The highest number the roll can be.</param>
        /// </summary>
        [SlashCommand("roll-to", "Roll a die to get a random number up to a custom maximum.")]
        public async Task RollUpToAsync(int max)
        {
            Random random = new Random((int)DateTime.UtcNow.Ticks);
            await FollowupAsync(text: $"神様 has blessed you with a {random.Next(1, max+1)}.", isTTS: false);
        }

        /// <summary>
        /// Roll a die inclusive between a custom minimum and maximum.
        /// <param name="min">The lowest number the roll can be.</param>
        /// <param name="max">The highest number the roll can be.</param>
        /// </summary>
        [SlashCommand("roll-between", "Roll a die to get a random number inclusive between a custom minimum and maximum.")]
        public async Task RollBetweenAsync(int min, int max)
        {
            if (min > max)
            {
                await FollowupAsync(text: "The minimum cannot be higher than the maximum.", isTTS: false, ephemeral: true);
                return;
            }

            Random random = new Random((int)DateTime.UtcNow.Ticks);
            await FollowupAsync(text: $"神様 has blessed you with a {random.Next(min, max+1)}.", isTTS: false);
        }

        /// <summary>
        /// Flip a coin at random.
        /// </summary>
        [SlashCommand("coinflip", "Flip a coin at random.")]
        public async Task CoinflipAsync()
        {
            Random random = new Random((int)DateTime.UtcNow.Ticks);
            var coin = Resources.coin_heads;
            var result = "heads";
            // Even is heads, odd is tails.
            if (random.Next(0, 100) % 2 == 1)
            {
                coin = Resources.coin_tails;
                result = "tails";
            }

            var ms = new MemoryStream();
            coin.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            await FollowupWithFileAsync(ms, $"{result}.png", null);
        }

        public enum PlannedMediaType
        {
            Anime,
            Either,
            Manga
        }

        /// <summary>
        /// Fetch a random Planned media entry for the user.
        /// <param name="mediaType">The type of media to fetch.</param>
        /// </summary>
        [SlashCommand("planned", "Fetch a random Planned media entry for the user.")]
        public async Task PlannedAsync([Summary(name: "media-type", description: "The type of media to randomize for")] PlannedMediaType mediaType = PlannedMediaType.Either)
        {
            bool includeAnime = true;
            bool includeManga = true;

            switch (mediaType)
            {
                case PlannedMediaType.Anime:
                    includeManga = false;
                    break;

                case PlannedMediaType.Manga:
                    includeAnime = false;
                    break;
            }

            await ChooseRandomPlannedItem(includeAnime, includeManga);
        }

        /// <summary>
        /// Choose a random planned media item for a specific user.
        /// </summary>
        /// <param name="includeAnime">Indicator whether to include anime media for the choice.</param>
        /// <param name="includeManga">Indicator whether to include manga media for the choice.</param>
        private async Task ChooseRandomPlannedItem(bool includeAnime = true, bool includeManga = true)
        {
            var user = await DatabaseUtility.GetInstance().GetSpecificUserAsync(Context.User.Id);
            if (user == null)
            {
                await FollowupAsync(text: "You need to tell me your Anilist before I can recommend you stuff!\n" +
                    "You can do this using the `setup anilist <ID/USERNAME>` command.", isTTS: false, ephemeral: true);
                return;
            }

            var planned = new List<MediaList>();

            if (includeAnime)
            {
                var animeList = await _aniListFetcher.FindPlannedUserList(user.AnilistId, MediaType.Anime.ToString());

                if (animeList.MediaListCollection.Lists.Count > 0)
                {
                    planned.AddRange(animeList.MediaListCollection.Lists[0].Entries);
                }
            }

            if (includeManga)
            {
                var mangaList = await _aniListFetcher.FindPlannedUserList(user.AnilistId, MediaType.Manga.ToString());

                if (mangaList.MediaListCollection.Lists.Count > 0)
                {
                    planned.AddRange(mangaList.MediaListCollection.Lists[0].Entries);
                }
            }

            if (planned.Count == 0)
            {
                var specificTypeInsert = "";

                // Determine whether to insert the media type in the message.
                if (!(includeAnime && includeManga))
                {
                    if (includeAnime)
                    {
                        specificTypeInsert = $"{MediaType.Anime} ";
                    } else
                    {
                        specificTypeInsert = $"{MediaType.Manga} ";
                    }
                }

                await FollowupAsync(text: $"You might want to add {specificTypeInsert}entries to Planning before asking me to choose one.", isTTS: false);
                return;
            }

            Random rand = new Random((int)DateTime.UtcNow.Ticks);

            var chosenOne = planned[rand.Next(planned.Count)];

            var consume = chosenOne.Media.Type == MediaType.Anime ? "watch" : "read";

            List<string> recommendMessages = new List<string>
            {
                $"I wholeheartedly recommend that you {consume} this one. It's one of my clone's favourites.",
                $"You totally should {consume} this. I heard that it's one of the best out there.",
                $"I cannot believe that you never got to {consume} this...",
                $"Why has nobody told you to {consume} this? It has such a reputation for a reason!",
                $"The favourite count on this one is wrong. It's going to have +1 after you finish {consume}ing it.",
                $"Go {consume} this right now. It's a classic for a reason.",
                "What have you been wasting your time on instead of this? Disgraceful.",
                $"If you {consume} this, I will fullfil one of your deepest desires. \uD83D\uDE18",
                $"You better {consume} this one, or I'm sending one of my clones after you.",
                "This is the next best thing after sliced bread and Shidou."
            };

            await FollowupAsync(text: recommendMessages[rand.Next(recommendMessages.Count)], isTTS: false, embed: _embedUtility.BuildAnilistMediaEmbed(chosenOne.Media));
        }
    }
}
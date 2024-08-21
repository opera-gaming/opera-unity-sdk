using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opera
{
    /// <summary>
    /// Request options for submitting the new score. See @"Opera.GxGames.ChallengeSubmitScore(System.Int32,Opera.GxGamesApiCallback{Opera.SubmitScoreData},Opera.ChallengeSubmitScoresOptions)?text=GxGames.ChallengeSubmitScore".
    /// </summary>
    public sealed class ChallengeSubmitScoresOptions
    {
        /// <summary>
        /// The challenge ID to use (if not specified, defaults to the currently active challenge).
        /// </summary>
        public string challengeId { get; }

        /// <param name="challengeId">The initial value for <see cref="challengeId"/>/></param>
        public ChallengeSubmitScoresOptions(string challengeId = "")
        {
            this.challengeId = challengeId;
        }
    }

    /// <summary>
    /// Request options for getting scores:
    /// * @"Opera.GxGames.ChallengeGetGlobalScores(Opera.GxGamesApiCallback{Opera.GetScoresData},Opera.ChallengeGetScoresOptions)?text=GxGames.ChallengeGetGlobalScores";
    /// * @"Opera.GxGames.ChallengeGetUserScores(Opera.GxGamesApiCallback{Opera.GetScoresData},Opera.ChallengeGetScoresOptions)?text=GxGames.ChallengeGetUserScores".
    /// </summary>
    public sealed class ChallengeGetScoresOptions
    {
        /// <summary>
        /// The page number to return.
        /// </summary>
        public int page { get; }

        /// <summary>
        /// The maximum number of items to return.
        /// </summary>
        public int pageSize { get; }

        /// <summary>
        /// The challenge ID to use (if not specified, defaults to the currently active challenge).
        /// </summary>
        public string challengeId { get; }

        /// <summary>
        /// The track ID to use (if not specified, defaults to the currently active track).
        /// </summary>
        public string trackId { get; }

        /// <param name="page">The initial value for <see cref="page"/></param>
        /// <param name="pageSize">The initial value for <see cref="pageSize"/></param>
        /// <param name="challengeId">The initial value for <see cref="challengeId"/></param>
        /// <param name="trackId">The initial value for <see cref="trackId"/></param>
        public ChallengeGetScoresOptions(
            int page = 0,
            int pageSize = 25,
            string challengeId = "",
            string trackId = ""
        )
        {
            this.page = page;
            this.pageSize = pageSize;
            this.challengeId = challengeId;
            this.trackId = trackId;
        }
    }

    /// <summary>
    /// Request options for getting challenges for the current game. See @"Opera.GxGames.ChallengeGetChallenges(Opera.GxGamesApiCallback{Opera.GetChallengesData},Opera.ChallengeGetChallengesOptions)?text=GxGames.ChallengeGetChallenges".
    /// </summary>
    public sealed class ChallengeGetChallengesOptions
    {
        /// <summary>
        /// The page number to return.
        /// </summary>
        public int page { get; }

        /// <summary>
        /// The maximum number of items to return.
        /// </summary>
        public int pageSize { get; }

        /// <summary>
        /// The track ID to use (if not specified, defaults to the currently active track).
        /// </summary>
        public string trackId { get; }

        /// <param name="page">The initial value for <see cref="page"/></param>
        /// <param name="pageSize">The initial value for <see cref="pageSize"/></param>
        /// <param name="trackId">The initial value for <see cref="trackId"/></param>
        public ChallengeGetChallengesOptions(
            int page = 0,
            int pageSize = 25,
            string trackId = ""
        )
        {
            this.page = page;
            this.pageSize = pageSize;
            this.trackId = trackId;
        }
    }
}

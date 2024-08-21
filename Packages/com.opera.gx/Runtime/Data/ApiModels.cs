using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Opera
{
    [System.Serializable]
    internal sealed class FullVersionResponseApi : GmxApiResult<FullVersionData> { }

    [System.Serializable]
    public sealed class FullVersionData
    {
        /// <summary>
        /// A flag indicating whether the full version of the game has been purchased.
        /// </summary>
        [Preserve] public bool isFullVersionPurchased;
    }

    [System.Serializable]
    internal sealed class SubmitScoreResponseApi : GmxApiResult<SubmitScoreData> { }

    [System.Serializable]
    public sealed class SubmitScoreData
    {
        /// <summary>
        /// A flag indicating whethre the submitted score is the new best score for the given challenge.
        /// </summary>
        [Preserve] public bool newBestScore;
    }

    [System.Serializable]
    internal sealed class GetScoresResponseApi : GmxApiResult<GetScoresData> { }

    [System.Serializable]
    public sealed class GetScoresData
    {
        [Preserve] public ChallengeData challenge;
        [Preserve] public ChallengeScoreData[] scores;
        [Preserve] public PaginationData pagination;
    }

    [System.Serializable]
    internal sealed class GetChallengesResponseApi : GmxApiResult<GetChallengesData> { }

    [System.Serializable]
    public sealed class GetChallengesData
    {
        [Preserve] public ChallengeData[] challenges;
        [Preserve] public PaginationData pagination;
    }

    [System.Serializable]
    internal sealed class PlayerDataResponseApi : GmxApiResult<PlayerData> { }

    [System.Serializable]
    public sealed class PlayerData
    {
        [Preserve] public string avatarUrl;
        [Preserve] public string userId;
        [Preserve] public string username;
    }

    [System.Serializable]
    public sealed class ChallengeData
    {
        /// <summary>
        /// The ID of the challenge.
        /// </summary>
        [Preserve] public string challengeId;

        /// <summary>
        /// A link to the cover art for the challenge.
        /// </summary>
        [Preserve] public string coverArt;

        /// <summary>
        /// The date and time when the challenge was created.
        /// </summary>
        [Preserve] public string creationDate;

        /// <summary>
        /// The type of the challenge (e.g., "duration").
        /// </summary>
        [Preserve] public string type;

        /// <summary>
        /// The winning criteria for the challenge (e.g., "lowest_wins").
        /// </summary>
        [Preserve] public string criteria;

        /// <summary>
        /// The date and time when the challenge starts (if timed).
        /// </summary>
        [Preserve] public string startsAt;

        /// <summary>
        /// The date and time when the challenge ends (if timed).
        /// </summary>
        [Preserve] public string endsAt;

        /// <summary>
        /// Has the challenge ended? (if timed).
        /// </summary>
        [Preserve] public bool hasEnded;

        /// <summary>
        /// Has the challenge started? (if timed).
        /// </summary>
        [Preserve] public bool hasStarted;

        /// <summary>
        /// Is the challenge published?
        /// </summary>
        [Preserve] public bool isPublished;

        /// <summary>
        /// Is the challenge timed?
        /// </summary>
        [Preserve] public bool isTimedChallenge;

        /// <summary>
        /// The short description of the challenge.
        /// </summary>
        [Preserve] public string shortDescription;

        /// <summary>
        /// The long description of the challenge.
        /// </summary>
        [Preserve] public string longDescription;

        /// <summary>
        /// The name of the challenge.
        /// </summary>
        [Preserve] public string name;

        /// <summary>
        /// The amount of players who have submitted scores for this challenge.
        /// </summary>
        [Preserve] public int players;
    }

    [System.Serializable]
    public sealed class ChallengeScoreData
    {
        /// <summary>
        /// The date and time when this score was submitted.
        /// </summary>
        [Preserve] public string achievementDate;

        /// <summary>
        /// The country code of the score submitter.
        /// </summary>
        [Preserve] public string countryCode;

        /// <summary>
        /// A struct containing the information about the submitter.
        /// </summary>
        [Preserve] public PlayerData player;

        /// <summary>
        /// The score value.
        /// </summary>
        [Preserve] public int score;

        /// <summary>
        /// The ID of the submitted score.
        /// </summary>
        [Preserve] public string scoreId;

    }

    [System.Serializable]
    public sealed class PaginationData
    {
        [Preserve] public int currPage;
        [Preserve] public int numPerPage;
        [Preserve] public int totalItems;
        [Preserve] public int totalPages;
    }
}

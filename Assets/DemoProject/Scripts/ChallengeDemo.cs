using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Opera
{
    public sealed class ChallengeDemo : MonoBehaviour
    {
        public InputField scoreField;
        public Button submitButton;

        public Button getGlobalScoresButton;
        public Button getUserScoresButton;
        public Button getChallengesButton;
        public Button getProfileInfoButton;

        public Toggle passNullOption;

        public InputField challengeIdField;
        public InputField pageField;
        public InputField pageSizeField;
        public InputField trackIdField;

        public Button logTopScoresButton;

        private void Awake()
        {
            // Simple examples to demonstrate making the requests.
            submitButton.onClick.AddListener(SubmitScore);
            getGlobalScoresButton.onClick.AddListener(GetGlobalScores);
            getUserScoresButton.onClick.AddListener(GetUserScores);
            getChallengesButton.onClick.AddListener(GetChallenges);
            getProfileInfoButton.onClick.AddListener(GetProfile);

            // A little more advanced example with multiple requests and extracting data.
            logTopScoresButton.onClick.AddListener(LogTopScoresForAllChallenges);
        }

        private void Start()
        {
            Debug.Log($"Current challenge ID: {GxGames.GetQueryParam("challenge")}");
        }

        private void SubmitScore()
        {
            GxGames.ChallengeSubmitScore(
                int.Parse(scoreField.text),
                options: ShouldPassNullOption ? null : new(GetChallengeId()),
                callback: (data, isOk, errorCodes) =>
                {
                    Debug.Log($"Request completed. Is OK: {isOk}. Error codes: {string.Join("; ", errorCodes)}");
                    Debug.Log($"Is new best score: {data.newBestScore}");
                });
        }

        private void GetGlobalScores()
        {
            GxGames.ChallengeGetGlobalScores(
                options: ShouldPassNullOption
                    ? null
                    : new(GetPage(), GetPageSize(), GetChallengeId(), GetTrackId()),
                callback: LogStringifiedData);
        }

        private void GetUserScores()
        {
            GxGames.ChallengeGetUserScores(
                options: ShouldPassNullOption
                    ? null
                    : new(GetPage(), GetPageSize(), GetChallengeId(), GetTrackId()),
                callback: LogStringifiedData);
        }

        private void GetChallenges()
        {
            GxGames.ChallengeGetChallenges(
                options: ShouldPassNullOption
                    ? null
                    : new(GetPage(), GetPageSize(), GetTrackId()),
                callback: LogStringifiedData);
        }

        private void GetProfile()
        {
            GxGames.ProfileGetInfo(
                callback: LogStringifiedData);
        }

        private void LogStringifiedData<TData>(TData data, bool isOk, string[] errorCodes)
        {
            Debug.Log($"Request completed. Is OK: {isOk}. Error codes: {string.Join("; ", errorCodes)}");
            Debug.Log($"Stringified data: {JsonUtility.ToJson(data, prettyPrint: true)}");
        }

        private string GetChallengeId() => challengeIdField.text;

        private int GetPage()
        {
            int pageNumber;
            int.TryParse(pageField.text, out pageNumber);
            return pageNumber;
        }

        private int GetPageSize()
        {
            int pageSize;
            int.TryParse(pageSizeField.text, out pageSize);
            return pageSize;
        }

        private string GetTrackId() => trackIdField.text;

        private bool ShouldPassNullOption => passNullOption.isOn;

        private void LogTopScoresForAllChallenges()
        {
            GxGames.ChallengeGetChallenges(
                callback: (data, isOk, errorCodes) =>
                {
                    if (!isOk)
                    {
                        Debug.LogError($"Failed to get challenges. Errors: {string.Join("; ", errorCodes)}");
                        return;
                    }

                    Debug.Log($"Challenges: {string.Join("; ", data.challenges.Select(challenge => challenge.name))}");

                    foreach(var challenge in data.challenges)
                    {
                        GetTopScoreFor(challenge);
                    }
                });
        }

        private void GetTopScoreFor(ChallengeData challenge)
        {
            GxGames.ChallengeGetGlobalScores(
                options: new(page: 0, pageSize: 1, challenge.challengeId),
                callback: (data, isOk, errorCodes) =>
                {
                    if (!isOk)
                    {
                        Debug.LogError($"Failed to get scores for challenge \"{challenge.name}\". Errors: {string.Join("; ", errorCodes)}");
                        return;
                    }

                    var topScore = data.scores.FirstOrDefault() ?? null;

                    var topScoreString = topScore == null
                        ? "<none>"
                        : $"{topScore.score} by {topScore.player.username}";

                    Debug.Log($"Top score for \"{challenge.name}\": {topScoreString}");
                });
        }
    }
}

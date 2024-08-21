using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Opera
{
    /// <summary>
    /// A singleton providing access to GxGames features.
    /// </summary>
    public static class GxGames
    {
        #region PRIVATE_FIELDS
        private readonly static PaymentTrigger paymentTrigger;
        private readonly static FullVersionPaymentStatusRequest fullVersionPaymentStatusRequest;
        private readonly static ChallengeSubmitScoreRequest challengeSubmitScoreRequest;
        private readonly static ChallengeGetScoresRequest challengeGetGlobalScoresRequest;
        private readonly static ChallengeGetScoresRequest challengeGetUserScoresRequest;
        private readonly static ChallengeGetChallengesRequest challengeGetChallengesRequest;
        private readonly static ProfileGetInfoRequest profileGetInfoRequest;

        private readonly static IServerSettings serverSettings;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets a value that indicates whether it is allowed to use GX.Games API.
        /// </summary>
        public static bool IsAllowed => Application.platform == RuntimePlatform.WebGLPlayer;
        #endregion

        #region COMMON
        /// <summary>
        /// Constructor.
        /// </summary>
        static GxGames()
        {
            if (!IsAllowed)
            {
                Debug.Log("GxGames API: Skipping initialization on a non-web platform.");
                return;
            }

            serverSettings = ServerSettingsFinder.FindServerSettings();

            paymentTrigger = new PaymentTrigger();
            fullVersionPaymentStatusRequest = new FullVersionPaymentStatusRequest(serverSettings);
            challengeSubmitScoreRequest = new ChallengeSubmitScoreRequest(serverSettings);
            challengeGetGlobalScoresRequest = new ChallengeGetScoresRequest(serverSettings, "scores");
            challengeGetUserScoresRequest = new ChallengeGetScoresRequest(serverSettings, "user-scores");
            challengeGetChallengesRequest = new ChallengeGetChallengesRequest(serverSettings);
            profileGetInfoRequest = new ProfileGetInfoRequest(serverSettings);
        }

        /// <summary>
        /// When your game is played on GX.Games, some extra parameters are passed into the URL of the
        /// game so they can be retrieved in-game.This function will return the value of the
        /// parameter specified in the argument as a string.
        /// </summary>
        /// <param name="param">The name of the parameter.</param>
        /// <returns>The value of the query parameter.</returns>
        /// <example>
        /// For example, if the game is open in a browser window and its address is `localhost:8000/?game=1234&amp;track=7890`, the following function: 
        /// ```csharp
        /// GetQueryParam('track')
        /// ```
        /// will return `7890`.
        /// </example>
        public static string GetQueryParam(string param) => IsAllowed ? JSGetQueryParam(param) : null;

        #endregion

        #region PAYMENTS
        /// <summary>
        /// Start the payment flow prompting the user to make a payment and making the necessary interactions with the server. 
        /// The callback `onPaymentCompleted` will be invoked. 
        /// 
        /// The callback `onPaymentCompleted` is invoked when the payment is
        /// completed or finished with the error.
        /// Payments work only when the game runs on GX.Games website. Testing making payments locally is not supported.
        /// Use the function @"Opera.GxGames.GetFullVersionPaymentStatus(Opera.GxGamesApiCallback{Opera.FullVersionData})?text=GetFullVersionPaymentStatus" 
        /// to check the purchase status after the payment is completed.
        /// </summary>
        /// <param name="id">The item ID.</param>
        /// <param name="onPaymentCompleted">
        /// This callback is invoked after the payment flow is completed. Pass a function to this callback to handle the received data.
        /// The callback is invoked whenever the payment is completed/failed or interrupted. This callback does not contain any information about the payment status. 
        /// You should trigger the function @"Opera.GxGames.GetFullVersionPaymentStatus(Opera.GxGamesApiCallback{Opera.FullVersionData})?text=GetFullVersionPaymentStatus" 
        /// to check if the user owns the full version: that is the only reliable way to ensure that the user now has the item.
        /// </param>
        public static void TriggerPayment(
        string id,
        PaymentCompletedCallback onPaymentCompleted
        ) => WithAssertions(() =>
            paymentTrigger.TriggerPayment(id, onPaymentCompleted));

        /// <summary>
        /// Receive the payment status for the game. This function is used to check whether the user has purchased the full version of the game. 
        /// The function makes an HTTP request to get this data from the backend. Once the data is received, the callback will be invoked.
        /// </summary>
        /// <param name="onPaymentStatusReceived">
        /// This callback is invoked after getting the payment status from the backend. Pass a function to this callback to handle the received data.
        /// </param>
        public static void GetFullVersionPaymentStatus(
            GxGamesApiCallback<FullVersionData> onPaymentStatusReceived
        ) => WithAssertions(() =>
            fullVersionPaymentStatusRequest.DoRequest(onPaymentStatusReceived));
        #endregion

        #region CHALLENGES
        /// <summary>
        /// This function is used to submit a new score to the currently active challenge, 
        /// for the user that is currently logged in. You specify the score value to submit to the challenge.
        /// When the request is completed, the callback will be invoked.
        /// You may also specify options for the request.
        /// </summary>
        /// <param name="score">The submitted score.</param>
        /// <param name="callback">Callback with the request result. Pass a function to this callback to handle the received data.</param>
        /// <param name="options">Request options.</param>
        public static void ChallengeSubmitScore(
            int score,
            GxGamesApiCallback<SubmitScoreData> callback,
            ChallengeSubmitScoresOptions options = null
        ) => WithAssertions(() =>
            challengeSubmitScoreRequest.DoRequest(score, callback, options));

        /// <summary>
        /// This function is used to retrieve all the scores for the currently active challenge. 
        /// Signing into GX.Games is not required for this function to work. When the request is completed, 
        /// the callback will be invoked.
        /// You may also specify options for the request.
        /// </summary>
        /// <param name="callback">Callback with the request result. Pass a function to this callback to handle the received data.</param>
        /// <param name="options">Request options.</param>
        public static void ChallengeGetGlobalScores(
            GxGamesApiCallback<GetScoresData> callback,
            ChallengeGetScoresOptions options = null
        ) => WithAssertions(() =>
            challengeGetGlobalScoresRequest.DoRequest(callback, options));

        /// <summary>
        /// This function is used to retrieve the current user's submitted scores for the currently active 
        /// challenge. Signing into GX.Games is required for this function to work. When the request is 
        /// completed, the callback will be invoked.
        /// </summary>
        /// <param name="callback">Callback with the request result. Pass a function to this callback to handle the received data.</param>
        /// <param name="options">Request options.</param>
        public static void ChallengeGetUserScores(
            GxGamesApiCallback<GetScoresData> callback,
            ChallengeGetScoresOptions options = null
        ) => WithAssertions(() =>
            challengeGetUserScoresRequest.DoRequest(callback, options));

        /// <summary>
        /// This function is used to retrieve all the challenges that have been created for the game on the currently active 
        /// track (or the one specified in options). When the request is completed, the callback will be invoked.
        /// </summary>
        /// <param name="callback">Callback with the request result. Pass a function to this callback to handle the received data.</param>
        /// <param name="options">Request options.</param>
        public static void ChallengeGetChallenges(
            GxGamesApiCallback<GetChallengesData> callback,
            ChallengeGetChallengesOptions options = null
        ) => WithAssertions(() =>
            challengeGetChallengesRequest.DoRequest(callback, options));
        #endregion

        #region PROFILE
        /// <summary>
        /// Get profile info. This function is used to retrieve information about the current user's profile. 
        /// The function makes an HTTP request to get this data from the backend. 
        /// Once the data is received, the callback will be invoked.
        /// </summary>
        /// <param name="callback">Callback with the request result. Pass a function to this callback to handle the received data.</param>
        public static void ProfileGetInfo(
            GxGamesApiCallback<PlayerData> callback
        ) => WithAssertions(() =>
            profileGetInfoRequest.DoRequest(callback));
        #endregion

        #region CLOUD_SAVES
        /// <summary>
        /// Generates a path to the folder for the save files in a GX.Games-compatible format. 
        /// The files which are located in this folder are managed by the cloud saves system. 
        /// The function returns the folder in the following format: `/userfs/<gameId>/`.  
        /// "userfs" is the name of the root folder which is used by Godot. "gameId" is the ID of the game on GX.Games.
        /// If the folder does not exist, the function creates it.
        /// 
        /// The function runs only on WebGL. If the platform is different, the function returns null.
        /// 
        /// The game ID is extracted from the query parameter `game` of the page URL where the game is running.
        /// If the page's URL does not contain this parameter, the function returns an empty string.
        /// </summary>
        /// <returns>The path to the folder.</returns>
        public static string GenerateDataPath()
        {
            if (!IsAllowed)
            {
                return null;
            }

            var gameId = JSGetQueryParam("game");

            if (string.IsNullOrEmpty(gameId))
            {
                return null;
            }
            else
            {
                var directory = Path.Combine("/idbfs", gameId);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return directory;
            }
        }
        #endregion

        #region PRIVATE_METHODS
        [DllImport("__Internal")]
        private static extern string JSGetQueryParam(string param);

        private static void WithAssertions(Action action)
        {
            if (!IsAllowed)
            {
                throw new Exception("GxGames API is not allowed on the current platform.");
            }

            action?.Invoke();
        }
        #endregion
    }
}

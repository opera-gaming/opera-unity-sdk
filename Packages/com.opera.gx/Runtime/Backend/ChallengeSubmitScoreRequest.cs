using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using System.Linq;

namespace Opera
{
    internal sealed class ChallengeSubmitScoreMessageListener : JsRequestMessageListener<SubmitScoreResponseApi, SubmitScoreData> { }

    internal sealed class ChallengeSubmitScoreRequest
    {
        private readonly IServerSettings serverSettings;

        public ChallengeSubmitScoreRequest(IServerSettings serverSettings)
        {
            this.serverSettings = serverSettings;
        }

        public void DoRequest(
            int score,
            GxGamesApiCallback<SubmitScoreData> callback,
            ChallengeSubmitScoresOptions options = null
        )
        {
            var messageListener = RuntimeRequestUtils.InstantiateJsMesageListener<ChallengeSubmitScoreMessageListener, SubmitScoreData>(callback);

            var gameId = GxGames.GetQueryParam("game");
            var challengeId = RuntimeRequestUtils.WithFallback(options?.challengeId, "challenge");
            var trackId = GxGames.GetQueryParam("track");

            var hash = Hash(gameId + challengeId + trackId + score.ToString());
            JSChallengeSubmitScore(messageListener.name, score, hash, serverSettings.ServerUrl, challengeId);
        }

        private static string Hash(string input)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        [DllImport("__Internal")]
        private static extern void JSChallengeSubmitScore(string callbackReceiver, int score, string hash, string apiDomain, string challengeId);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Opera
{
    internal sealed class ChallengeGetScoresMessageListener : JsRequestMessageListener<GetScoresResponseApi, GetScoresData> { }

    internal sealed class ChallengeGetScoresRequest
    {
        private readonly IServerSettings serverSettings;
        private readonly string endpontEnding;

        public ChallengeGetScoresRequest(IServerSettings serverSettings, string endpontEnding)
        {
            this.serverSettings = serverSettings;
            this.endpontEnding = endpontEnding;
        }

        public void DoRequest(
            GxGamesApiCallback<GetScoresData> callback,
            ChallengeGetScoresOptions options = null)
        {
            var messageListener = RuntimeRequestUtils.InstantiateJsMesageListener<ChallengeGetScoresMessageListener, GetScoresData>(callback);

            JSChallengeGetScores(
                messageListener.name,
                serverSettings.ServerUrl,
                endpontEnding,
                page: options?.page ?? 0,
                pageSize: options?.pageSize ?? 25,
                challengeId: RuntimeRequestUtils.WithFallback(options?.challengeId, "challenge"),
                trackId: RuntimeRequestUtils.WithFallback(options?.trackId, "track")
                );
        }

        [DllImport("__Internal")]
        private static extern void JSChallengeGetScores(
            string callbackReceiver,
            string apiDomain,
            string endpontEnding,
            int page,
            int pageSize,
            string challengeId,
            string trackId);
    }
}

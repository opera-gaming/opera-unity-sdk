using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Opera
{
    internal sealed class ChallengeGetChallengesMessageListener : JsRequestMessageListener<GetChallengesResponseApi, GetChallengesData> { }

    internal sealed class ChallengeGetChallengesRequest
    {
        private readonly IServerSettings serverSettings;

        public ChallengeGetChallengesRequest(IServerSettings serverSettings)
        {
            this.serverSettings = serverSettings;
        }

        public void DoRequest(
            GxGamesApiCallback<GetChallengesData> callback,
            ChallengeGetChallengesOptions options = null
        )
        {
            var messageListener = RuntimeRequestUtils.InstantiateJsMesageListener<ChallengeGetChallengesMessageListener, GetChallengesData>(callback);

            JSChallengeGetChallenges(
                messageListener.name,
                apiDomain: serverSettings.ServerUrl,
                page: options?.page ?? 0,
                pageSize: options?.pageSize ?? 25,
                trackId: RuntimeRequestUtils.WithFallback(options?.trackId, "track")
            );
        }

        [DllImport("__Internal")]
        private static extern void JSChallengeGetChallenges(string callbackReceiver, string apiDomain, int page, int pageSize, string trackId);
    }
}

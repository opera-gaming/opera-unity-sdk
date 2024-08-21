using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Opera
{
    internal sealed class ProfileGetInfoMessageListener : JsRequestMessageListener<PlayerDataResponseApi, PlayerData> { }

    internal sealed class ProfileGetInfoRequest
    {
        private readonly IServerSettings serverSettings;

        public ProfileGetInfoRequest(IServerSettings serverSettings)
        {
            this.serverSettings = serverSettings ?? throw new System.ArgumentNullException(nameof(serverSettings));
        }

        public void DoRequest(GxGamesApiCallback<PlayerData> callback)
        {
            var messageListener = RuntimeRequestUtils.InstantiateJsMesageListener<ProfileGetInfoMessageListener, PlayerData>(callback);

            JSChallengeGetProfileInfo(messageListener.name, serverSettings.ServerUrl);
        }

        [DllImport("__Internal")]
        private static extern void JSChallengeGetProfileInfo(string callbackReceiver, string apiDomain);
    }
}

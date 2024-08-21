using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Opera
{
    internal sealed class FullVersionPaymentStatusMessageListener : JsRequestMessageListener<FullVersionResponseApi, FullVersionData> { }

    internal sealed class FullVersionPaymentStatusRequest
    {
        private readonly IServerSettings serverSettings;

        public FullVersionPaymentStatusRequest(IServerSettings serverSettings)
        {
            this.serverSettings = serverSettings;
        }

        public void DoRequest(GxGamesApiCallback<FullVersionData> callback)
        {
            var messageListener = RuntimeRequestUtils.InstantiateJsMesageListener<FullVersionPaymentStatusMessageListener, FullVersionData>(callback);

            JsGetFullVersionPaymentStatus(messageListener.name, serverSettings.ServerUrl);
        }

        [DllImport("__Internal")]
        private static extern void JsGetFullVersionPaymentStatus(string callbackReceiver, string apiDomain);
    }
}

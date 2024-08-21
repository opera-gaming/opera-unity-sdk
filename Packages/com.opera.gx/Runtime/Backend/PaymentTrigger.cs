using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Opera
{
    internal sealed class PaymentTrigger
    {
        private int requestIndex = 0;

        public void TriggerPayment(string id, PaymentCompletedCallback onPaymentCompleted)
        {
            var callbackListener = InstantiateJsCallbackListener(onPaymentCompleted);
            JsTriggerPayment(id, callbackListener.name);
        }

        [DllImport("__Internal")]
        private static extern void JsTriggerPayment(string id, string callbackReceiver);

        private PaymentCompletionListener InstantiateJsCallbackListener(PaymentCompletedCallback callback)
        {
            var component =
                new GameObject($"GxGames PaymentCallbackListener {requestIndex++}")
                .AddComponent<PaymentCompletionListener>();

            component.SetCallback(callback);

            return component;
        }
    }
}

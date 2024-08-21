using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opera
{
    internal sealed class PaymentCompletionListener : MonoBehaviour
    {
        PaymentCompletedCallback callback;

        public void SetCallback(PaymentCompletedCallback callback)
        {
            this.callback = callback;
        }

        // A message to be invoked from JavaScript.
        internal void OnPaymentCompleted(string id)
        {
            callback?.Invoke(id);

            Destroy(gameObject);
        }
    }
}

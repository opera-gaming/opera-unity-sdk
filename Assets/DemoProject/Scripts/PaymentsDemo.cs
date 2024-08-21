using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opera
{
    public sealed class PaymentsDemo : MonoBehaviour
    {
        public GameObject errorIndicator;
        public GameObject isNotPurchasedIndicator;
        public GameObject isPurchasedIndicator;
        public CanvasGroup indicatorsGroup;

        private void Awake()
        {
            CheckPayment();
        }

        public void TriggerPayment()
        {
            IndicateAwaiting(true);

            GxGames.TriggerPayment("test-id", (id) =>
            {
                Debug.Log($"Payment completed for {id}");
                CheckPayment();
            });
        }

        public void CheckPayment()
        {
            if (!GxGames.IsAllowed) return;

            IndicateAwaiting(true);
            
            GxGames.GetFullVersionPaymentStatus((data, isOk, errorCodes) =>
            {
                DeactivateAllIndicators();
                IndicatorToActivate(data?.isFullVersionPurchased ?? false, isOk, errorCodes).SetActive(true);

                if (errorCodes.Length > 0)
                {
                    Debug.LogError($"Error codes: {string.Join("; ", errorCodes)}");
                }

                IndicateAwaiting(false);
            });
        }

        private GameObject IndicatorToActivate(bool isFullVersionPurchased, bool isOk, string[] errorCodes) =>
            (isFullVersionPurchased, isOk, errorCodes) switch
            {
                { isFullVersionPurchased: true } => isPurchasedIndicator,
                { isOk: true } => isNotPurchasedIndicator,
                _ => errorIndicator,
            };

        private void DeactivateAllIndicators()
        {
            var allIndicators = new[] { errorIndicator, isNotPurchasedIndicator, isPurchasedIndicator };

            foreach (var indicator in allIndicators)
            {
                indicator.SetActive(false);
            }
        }

        private void IndicateAwaiting(bool isAwaiting) => indicatorsGroup.alpha = isAwaiting ? 0.5f : 1;
    }
}

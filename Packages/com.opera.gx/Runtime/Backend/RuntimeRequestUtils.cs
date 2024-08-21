using UnityEngine;

namespace Opera
{
    internal static class RuntimeRequestUtils
    {
        private static int requestIndex = 0;

        public static TComponent InstantiateJsMesageListener<TComponent, TData>(GxGamesApiCallback<TData> callback)
            where TComponent: MonoBehaviour, IJsRequestMessageListener<TData>
        {
            var component = new GameObject($"GxGames JsRequestMessageListener {requestIndex++}").AddComponent<TComponent>();
            component.SetCallback(callback);

            return component;
        }

        public static string WithFallback(string value, string fallbackQueryParameterName)
        {
            return string.IsNullOrEmpty(value)
                ? GxGames.GetQueryParam(fallbackQueryParameterName)
                : value;
        }
    }
}

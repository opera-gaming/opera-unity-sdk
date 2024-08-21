using UnityEngine;

namespace Opera
{
    internal interface IJsRequestMessageListener<TData>
    {
        void SetCallback(GxGamesApiCallback<TData> callback);
    }

    internal abstract class JsRequestMessageListener<TApiResult, TData> : MonoBehaviour, IJsRequestMessageListener<TData>
        where TApiResult : GmxApiResult<TData>
    {
        private GxGamesApiCallback<TData> callback;
        private static GxResponseParser<TApiResult, TData> responseParser = new GxResponseParser<TApiResult, TData>(Debug.LogError);

        public void SetCallback(GxGamesApiCallback<TData> callback)
        {
            this.callback = callback;
        }

        // A message to be invoked from JavaScript.
        public void Parse(string response)
        {
            var (data, isOk, errors) = responseParser.Parse(response);
            callback?.Invoke(data, isOk, errors);

            Destroy(gameObject);
        }

        // A message to be invoked from JavaScript.
        public void HandleUnknownError()
        {
            callback?.Invoke(default, false, new string[0]);

            Destroy(gameObject);
        }
    }
}

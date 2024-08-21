using System;
using System.Linq;
using UnityEngine;

namespace Opera
{
    internal sealed class GxResponseParser<TApiResult, TData> where TApiResult : GmxApiResult<TData>
    {
        private readonly Action<object> logError;

        public GxResponseParser(Action<object> logError)
        {
            this.logError = logError;
        }

        public (TData data, bool isOk, string[] errors) Parse(string response)
        {
            try
            {
                var responseObject = JsonUtility.FromJson<TApiResult>(response);
                var isOk = responseObject.errors != null && responseObject.errors.Length == 0;

                var errorCodes = responseObject.errors == null ? new string[0] :
                                 responseObject.errors
                                                    .Select(error => error?.code)
                                                    .Where(code => code != null)
                                                    .ToArray();

                return (isOk ? responseObject.Data : default, isOk, errorCodes);
            }
            catch (Exception exception)
            {
                // Using a function from variable instead of Debug.LogError. Otherwise
                // this line of code breaks automatic tests.
                logError(
                    "GxGames: Could not parse the response.\n" +
                    "Response: " + response + "\n" +
                    "Exception: " + exception);
                return (default, false, new string[0]);
            }
        }
    }
}

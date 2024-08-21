using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Opera
{
    public class OperaGxGetRequest : IOperaGxGetRequest
    {
        private readonly IJsonUtility jsonUtility;
        private readonly IUserInterface userInterface;

        public OperaGxGetRequest(IJsonUtility jsonUtility, IUserInterface userInterface)
        {
            this.jsonUtility = jsonUtility ?? throw new ArgumentNullException(nameof(jsonUtility));
            this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
        }

        /// <summary>
        /// Make a GET request to an Opera server
        /// </summary>
        /// <param name="requestPath">Request without domain name. Should not have a slash in the beginning.</param>
        /// <returns>Returns a string response from Opera GX server.</returns>
        public (bool success, TData data) Get<TResult, TData>(string OAUTH2_access_token, string serverURL, string requestPath)
            where TResult : GmxApiResult<TData>
        {
            try
            {
                var request = serverURL + requestPath;
                var listRequest = HttpWebRequest.Create(request) as HttpWebRequest;
                listRequest.Method = "GET";
                listRequest.Headers.Add("Authorization", string.Format("Bearer {0}", OAUTH2_access_token));

                var listTokenRequest = (HttpWebResponse)listRequest.GetResponse();

                var serverResponse = "";

                using (var responseStream = listTokenRequest.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        serverResponse = reader.ReadToEnd();
                    } // end using
                } // end using

                return (true, jsonUtility.FromJson<TResult>(serverResponse).data);
            }
            catch (WebException _ex)
            {
                if (_ex.Response != null)
                {
                    if (_ex.Response is HttpWebResponse hrm)
                    {
                        userInterface.Log($"Request Error {(int)hrm.StatusCode}: {hrm.StatusCode}.");
                    }

                    var serverResponse = "";

                    using (var responseStream = _ex.Response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            serverResponse = reader.ReadToEnd();
                        } // end using
                    } // end using

                    var errors = jsonUtility.FromJson<GameResponseApi>(serverResponse)?.errors;
                    // TODO: convert error codes into messages
                    userInterface.Log($"Error on fetching from {requestPath}: " + string.Join("; ", errors?.Select(error => error.code) ?? new string[0]));
                } // end if
                else
                {
                    userInterface.LogError($"Error on fetching from {requestPath}. {_ex}");
                }

                return (false, default(TData));
            }
            catch (Exception exception)
            {
                userInterface.LogError($"Error on fetching from {requestPath}. {exception}");
                return (false, default(TData));
            }
        }
    }
}

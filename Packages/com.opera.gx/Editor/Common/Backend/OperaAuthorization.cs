using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Opera
{
    /// <summary>
    /// A class for authorization.
    /// </summary>
    public sealed class OperaAuthorization
    {
        public string OAUTH2_access_token => sessionStorage.OAUTH2_access_token;

        private string m_OAUTH2_refresh_token => sessionStorage.OAUTH2_refresh_token;

        private DateTime m_expiry => sessionStorage.Expiry;
        private Random m_random = new Random();

        private const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const string responseType = "code";
        private const string clientSecret = "";

        private string authHost => $"{oauthServer}authorize/";
        private string tokenHost => $"{oauthServer}token/";
        private string oauthServer => serverSettings.OauthServer;

        private readonly IServerSettings serverSettings;
        private readonly string scope;
        private readonly string redirect;
        private readonly string clientID;
        private readonly ISessionStorage sessionStorage;
        private readonly IJsonUtility jsonUtility;
        private readonly IUserInterface userInterface;
        private readonly IHttpRequest httpRequest;
        private readonly UrlOpener urlOpener;
        private readonly IApplicationFocusStrategy applicationFocuser;

        public OperaAuthorization(
            IServerSettings serverSettings,
            string scope,
            string redirect,
            string clientID,
            ISessionStorage sessionStorage,
            IJsonUtility jsonUtility,
            IUserInterface userInterface,
            IHttpRequest httpRequest,
            UrlOpener urlOpener,
            IApplicationFocusStrategy applicationFocuser)
        {
            this.serverSettings = serverSettings ?? throw new ArgumentNullException(nameof(serverSettings));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.redirect = redirect ?? throw new ArgumentNullException(nameof(redirect));
            this.clientID = clientID ?? throw new ArgumentNullException(nameof(clientID));
            this.sessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
            this.jsonUtility = jsonUtility ?? throw new ArgumentNullException(nameof(jsonUtility));
            this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
            this.httpRequest = httpRequest ?? throw new ArgumentNullException(nameof(httpRequest));
            this.urlOpener = urlOpener ?? throw new ArgumentNullException(nameof(urlOpener));
            this.applicationFocuser = applicationFocuser ?? throw new ArgumentNullException(nameof(applicationFocuser));
        }

        public bool OAUTH2_Reauthorise_Act()
        {
            bool ret = false;
            if (DateTime.Now > m_expiry)
            {
                if (!OAUTH2_RefreshToken())
                {
                    // if refresh failes then do the full auth process.
                    OAUTH2_GetToken();
                } // end if
            } // end if
            if (!string.IsNullOrEmpty(OAUTH2_access_token))
            {
                ret = true;
            } // end if
            return ret;
        }

        private string GetNonce()
        {
            return new string(Enumerable.Repeat(allowedChars, 32).Select(s => s[m_random.Next(s.Length)]).ToArray());
        }

        public bool OAUTH2_GetToken()
        {
            var stateNonce = GetNonce();
            string uri = string.Format("{0}?response_type={1}&client_id={2}&redirect_uri={3}&state={4}&scope={5}", authHost, responseType, clientID, redirect, stateNonce, scope);
            string code = string.Empty;
            WaitHandle handle = new AutoResetEvent(false);
            bool fExitingEarly = false;

            // we need to spin up the web hook
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(redirect);
            listener.Start();
            IAsyncResult resultListener = listener.BeginGetContext(new AsyncCallback((result) =>
            {
                if (!fExitingEarly)
                {
                    HttpListener listenerI = (HttpListener)result.AsyncState;

                    HttpListenerContext context = listenerI.EndGetContext(result);
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    // need to get the code from the RawURL
                    var query = request.Url.Query.Replace("?", "").Split('&').ToDictionary(pair => pair.Split('=').First(), pair => pair.Split('=').Last());
                    if (query.ContainsKey("code"))
                    {
                        code = query["code"];


                        // write back the response
                        string responseString = "Ok. OAuth2 received... please close this tab and go back to the game engine.";
                        byte[] responseStringBuff = System.Text.Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = responseStringBuff.Length;

                        try
                        {
                            response.OutputStream.Write(responseStringBuff, 0, responseStringBuff.Length);
                            response.OutputStream.Close();
                        }
                        catch
                        {
                            // The output stream may become disposed. I caught it in Godot only by this moment but let it be here as well just in case.
                            userInterface.LogWarning("Could not display the response on the authorization page. The error is not critical, you may continue.");
                        }
                    } // end block
                    else
                    {
                        if (query.ContainsKey("error") && query.ContainsKey("error_description"))
                        {
                            userInterface.Log(string.Format("Error received {0} - {1}", query["error"], query["error_description"]));
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("{ ");
                            foreach (var kvp in query)
                            {
                                sb.AppendFormat("{0} : {1}, ", kvp.Key, kvp.Value);
                            } // end foreach
                            sb.Append(" }");
                            userInterface.Log(string.Format("Unknown Error received {0}", sb.ToString())); ;
                        } // end else
                    }
                    // signal that we now have the "code"from the OAuth results
                    ((AutoResetEvent)handle).Set();

                    applicationFocuser.FocusEditorWindow();
                } // end if
            }), listener);

            applicationFocuser.OnBeforeUnfocusing();
            urlOpener.OpenOperaGXBrowser(uri);

            // loop while we are not signaled
            userInterface.OnProgressBegin("Authorization", "Waiting for an action from you in the browser...");

            bool fExited = false;
            int nTimeout = 10 * 1000 * 1000;
            while (!resultListener.AsyncWaitHandle.WaitOne(1000))
            {
                if (userInterface.DisplayCancelableProgressBar("Authorization", "Waiting for an action from you in the browser...", -1f))
                {
                    userInterface.Log("Authorization has been cancelled.");
                    fExited = true;
                    break;
                }

                nTimeout -= 1000;
                if (nTimeout <= 0)
                {
                    userInterface.Log("Timeout for authorization is over.");
                    break;
                } // end if
            } // end while

            userInterface.OnProgressEnd();

            listener.Stop();
            listener.Close();

            if (fExited || (nTimeout <= 0))
            {
                // signal that we are exiting early
                fExitingEarly = true;
                return false;
            } // end if
            else
            {
                handle.WaitOne();

                if (!string.IsNullOrEmpty(code))
                {
                    return OAUTH2_Authorisation(tokenHost, redirect, clientID, clientSecret, scope, code);
                } // end if
                else
                {
                    return false;
                }
            } // end else
        }

        private bool OAUTH2_RefreshToken()
        {
            string serverResponse;
            bool ret = false;
            try
            {
                string postData = string.Format("grant_type=refresh_token&refresh_token={0}&client_id={1}&client_secret{2}",
                                m_OAUTH2_refresh_token, clientID, clientSecret);

                serverResponse = httpRequest.Post(url: tokenHost, contentType: "application/x-www-form-urlencoded", postData: postData);

                HandleOAuthTokenResponse(serverResponse);
                ret = true;
            } // end try
            catch (Exception _ex)
            {
                userInterface.Log($"Exception on token refreshing. {_ex}");
            } // end catch

            return ret;
        } // end OAUTH2_Old_RefreshToken

        private bool OAUTH2_Authorisation(string tokenHost, string redirect, string clientID, string clientSecret, string scope, string code)
        {
            string serverResponse;
            bool ret = false;
            try
            {
                string postData = string.Format("grant_type=authorization_code&code={0}&redirect_uri={1}&scope={2}&client_id={3}&client_secret{4}",
                                code, redirect, scope, clientID, clientSecret);

                serverResponse = httpRequest.Post(url: tokenHost, contentType: "application/x-www-form-urlencoded", postData: postData);
                HandleOAuthTokenResponse(serverResponse);

                ret = true;
            } // end try
            catch (Exception _ex)
            {
                userInterface.Log($"Exception on authorization. {_ex}");
            } // end catch

            return ret;
        } // end OAUTH2_Authorisation

        private void HandleOAuthTokenResponse(string serverResponse)
        {
            var parsedResponse = jsonUtility.FromJson<AuthorizationResponse>(serverResponse);

            sessionStorage.OAUTH2_access_token = parsedResponse?.access_token ?? string.Empty;
            sessionStorage.OAUTH2_refresh_token = parsedResponse?.refresh_token ?? string.Empty;
            sessionStorage.OAUTH2_expires_in = parsedResponse?.expires_in ?? default;

            sessionStorage.Expiry = DateTime.Now + new TimeSpan(0, 0, sessionStorage.OAUTH2_expires_in);
        } // end HandleOAuthTokenResponse
    }

    [Serializable]
    public sealed class AuthorizationResponse
    {
        public string access_token;
        public string token_id;
        public string refresh_token;
        public string scope;
        public string token_type;
        public int expires_in;
    }
}

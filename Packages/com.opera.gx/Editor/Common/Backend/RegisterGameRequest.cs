using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Opera
{
    /// <summary>
    /// A class for authorization.
    /// </summary>
    public class RegisterGameRequest
    {
        private readonly OperaAuthorization operaAuthorization;
        private readonly IJsonUtility jsonUtility;
        private readonly IUserInterface userInterface;
        private readonly IHttpRequest httpRequest;
        private readonly string engineAlias;

        public RegisterGameRequest(OperaAuthorization operaAuthorization, IJsonUtility jsonUtility, IUserInterface userInterface, IHttpRequest httpRequest, string engineAlias)
        {
            this.operaAuthorization = operaAuthorization ?? throw new ArgumentNullException(nameof(operaAuthorization));
            this.jsonUtility = jsonUtility ?? throw new ArgumentNullException(nameof(jsonUtility));
            this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
            this.httpRequest = httpRequest ?? throw new ArgumentNullException(nameof(httpRequest));
            this.engineAlias = engineAlias ?? throw new ArgumentNullException(nameof(engineAlias));
        }

        public GameDataApi GXC_Create(string serverURL, string gameName, string groupId)
        {
            var serverResponse = string.Empty;
            try
            {
                string postData = $"{{ " +
                    $"\"name\": \"{gameName}\", " +
                    $"\"studioId\": \"{groupId}\", " +
                    $"\"gameEngine\": \"{engineAlias}\" " +
                $"}}";

                serverResponse = httpRequest.Post(
                    url: string.Format("{0}gamedev/games", serverURL),
                    contentType: "application/json",
                    authorization: string.Format("Bearer {0}", operaAuthorization.OAUTH2_access_token),
                    accept: "*/*",
                    postData: postData);

                return jsonUtility.FromJson<GameResponseApi>(serverResponse).data;
            } // end try
            catch (WebException _ex)
            {
                if (_ex.Response != null)
                {
                    if (_ex.Response is HttpWebResponse hrm)
                    {
                        userInterface.Log($"Request Error {(int)hrm.StatusCode}: {hrm.StatusCode}.");
                    }

                    using (var responseStream = _ex.Response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            serverResponse = reader.ReadToEnd();
                        } // end using
                    } // end using

                    var errors = jsonUtility.FromJson<GameResponseApi>(serverResponse)?.errors;
                    // TODO: convert error codes into messages
                    userInterface.Log("Error on registering the new game: " + string.Join("; ", errors?.Select(error => error.code) ?? new string[0]));
                } // end if
                else
                {
                    userInterface.LogError($"Unrecognized error on registering the new game. {_ex}");
                }
            } // end catch
            catch (Exception _ex)
            {
                userInterface.LogError($"Unrecognized error on registering the new game. {_ex}");
            } // end catch

            return default;
        } // end GXC_Create
    }
}


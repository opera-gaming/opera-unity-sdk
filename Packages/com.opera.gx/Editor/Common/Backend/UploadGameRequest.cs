using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Opera
{
    public sealed class UploadGameRequest : IUploadGameRequest
    {
        private const int PROGRESS_BAR_UPDATE_PERIOD_MILLISECONDS = 200;

        private readonly IJsonUtility jsonUtility;
        private readonly IUserInterface userInterface;

        public UploadGameRequest(IJsonUtility jsonUtility, IUserInterface userInterface)
        {
            this.jsonUtility = jsonUtility ?? throw new ArgumentNullException(nameof(jsonUtility));
            this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
        }

        public GameDataApi UploadGame(string OAUTH2_access_token, string gameId, string version, string serverURL, string filename)
        {
            bool ret = false;
            HttpClient client = new HttpClient();

            // The default timeout is 100 seconds. Without this line, the uploading may be aborted in the middle of the process.
            client.Timeout = Timeout.InfiniteTimeSpan;

            client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", OAUTH2_access_token));
            var content = new MultipartFormDataContent();
            byte[] contentBytes = File.ReadAllBytes(filename);

            var progressValue = 0f;
            var isContentStreamInterrupted = false;

            using (var ms = new MemoryStream(contentBytes))
            {
                var contentStream = new ProgressableStreamContent(new StreamContent(ms), (sent, total) =>
                {
                    float percentage = ((float)sent * 100.0f) / (float)total;
                    progressValue = percentage / 100.0f;
                });
                contentStream.OnContentReadingInterrupted = () => isContentStreamInterrupted = true;

                contentStream.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
                content.Add(contentStream, "file", Path.GetFileName(filename));

                var requestUri = $"{serverURL}gamedev/games/{gameId}/bundles?version={version}";

                var cancelTokenSource = new CancellationTokenSource();

                var task = client.PostAsync(requestUri, content, cancelTokenSource.Token);

                try
                {
                    userInterface.OnProgressBegin("Uploading", "Uploading...");

                    while (!task.Wait(PROGRESS_BAR_UPDATE_PERIOD_MILLISECONDS))
                    {
                        if (userInterface.DisplayCancelableProgressBar("Uploading", "Uploading...", progressValue))
                        {
                            cancelTokenSource.Cancel();
                            userInterface.Log("Uploading has been cancelled.");
                            break;
                        }

                        if (isContentStreamInterrupted)
                        {
                            userInterface.LogError("Uploading stream has been interrupted");
                            break;
                        }
                    } // end while

                    userInterface.OnProgressEnd();

                    HttpResponseMessage hrm = null;

                    hrm = task.Result;
                    ret = hrm.IsSuccessStatusCode;
                    string serverResponse = hrm.Content.ReadAsStringAsync().Result;

                    if (ret)
                    {
                        userInterface.Log("Successfully uploaded to GX.Games");
                        return jsonUtility.FromJson<GameResponseApi>(serverResponse).data;
                    } // end if
                    else
                    {
                        userInterface.Log($"Request Error {(int)hrm.StatusCode}: {hrm.StatusCode}.");

                        var errors = jsonUtility.FromJson<GameResponseApi>(serverResponse)?.errors;
                        // TODO: convert error codes into messages
                        userInterface.LogError($"Error on uploading to GX.Games: {string.Join("; ", errors?.Select(error => error.code) ?? new string[0])}");
                    } // end else
                }
                catch (Exception e)
                {
                    if (e is AggregateException aggregateException &&
                        aggregateException.InnerExceptions.Count == 1 &&
                        aggregateException.InnerException is TaskCanceledException)
                    {
                        return null;
                    }

                    userInterface.LogError($"Unrecognized error on uploading to GX.Games: {e}");
                }
                return null;
            }
        }
    }

}


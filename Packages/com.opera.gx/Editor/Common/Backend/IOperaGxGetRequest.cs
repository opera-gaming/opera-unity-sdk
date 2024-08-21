namespace Opera
{
    public interface IOperaGxGetRequest
    {
        (bool success, TData data) Get<TResult, TData>(string OAUTH2_access_token, string serverURL, string requestPath) where TResult : GmxApiResult<TData>;
    }
}
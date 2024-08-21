namespace Opera
{
    public interface IUploadGameRequest
    {
        GameDataApi UploadGame(string OAUTH2_access_token, string gameId, string version, string serverURL, string filename);
    }
}
namespace Opera
{
    public sealed class DefaultServerSettings : IServerSettings
    {
        public string ServerUrl => "https://api.gxc.gg/";
        public string OauthServer => "https://oauth2.opera-api.com/oauth2/v1/";
    }
}

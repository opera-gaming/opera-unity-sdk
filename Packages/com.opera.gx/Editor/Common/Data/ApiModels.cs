namespace Opera
{
    [System.Serializable]
    public sealed class GroupsResponseApi : GmxApiResult<GroupsDataApi> { }

    [System.Serializable]
    public sealed class GroupsDataApi
    {
        public GroupDataApi[] studios;
    }

    [System.Serializable]
    public sealed class GroupDataApi
    {
        public string name;
        public string studioId;
        public int gameMaxSize;
    }

    [System.Serializable]
    public sealed class GamesResponseApi : GmxApiResult<GamesDataApi> { }

    [System.Serializable]
    public sealed class GameResponseApi : GmxApiResult<GameDataApi> { }

    [System.Serializable]
    public sealed class GamesDataApi
    {
        public GameDataApi[] games;
    }

    [System.Serializable]
    public class GameDataApi
    {
        public string title;
        public string editUrl;
        public GroupDataApi studio;
        public string gameId;
        public string internalShareUrl;
        public string publicShareUrl;
        public string version;
    }

    [System.Serializable]
    public sealed class ProfileResponseApi : GmxApiResult<ProfileDataApi> { }

    [System.Serializable]
    public class ProfileDataApi
    {
        public string username;
    }

    [System.Serializable]
    public sealed class GameMaximumSizeResponseApi : GmxApiResult<GameMaximumSizeApi> { }

    [System.Serializable]
    public class GameMaximumSizeApi
    {
        public long gameMaxSize;
    }
}

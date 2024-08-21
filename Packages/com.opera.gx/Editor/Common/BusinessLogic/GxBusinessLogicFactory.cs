namespace Opera
{
    public static class GxBusinessLogicFactory
    {
        private const string SCOPE = "https://api.gx.games/gamedev:write+https://api.gx.games/gamedev:read";

        /// <summary>
        /// This factory methods accepts engine-specific dependencies as arguments.
        /// </summary>
        /// <returns></returns>
        public static IGxBusinessLogic CreateBusinessLogic(
            IJsonUtility jsonUtility,
            IUserInterface userInterface,
            IBuilder builder,
            ILocalGameData gameDataStorage, 
            ISessionStorage sessionStorage,
            IServerSettings serverSettings,
            IHttpRequest httpRequest,
            IOperaGxGetRequest operaGxGetRequest,
            IUploadGameRequest uploadRequest,
            IApplicationFocusStrategy applicationFocuser,
            string redirect,
            string clientID,
            string engineAlias
        )
        {
            var gamesSessionStorage = new SessionGamesData(sessionStorage);
            var groupsSessionStorage = new SessionGroupsData(sessionStorage);
            var profileSessionStorage = new SessionProfileData(sessionStorage);

            var urlOpener = new UrlOpener(userInterface);

            var authorization = new OperaAuthorization(serverSettings, SCOPE, redirect, clientID, sessionStorage, jsonUtility, userInterface, httpRequest, urlOpener, applicationFocuser);

            var registerGameRequest = new RegisterGameRequest(authorization, jsonUtility, userInterface, httpRequest, engineAlias);

            var allGamesCachedData = new CachedCloudData<GamesResponseApi, GamesDataApi, GameDataApi[]>(operaGxGetRequest, authorization, serverSettings, "gamedev/games?pageSize=1000", gamesSessionStorage, data => data.games);
            var allGroupsCachedData = new CachedCloudData<GroupsResponseApi, GroupsDataApi, GroupDataApi[]>(operaGxGetRequest, authorization, serverSettings, "gamedev/studios?pageSize=1000", groupsSessionStorage, data => data.studios);
            var profileCachedData = new CachedCloudData<ProfileResponseApi, ProfileDataApi, ProfileDataApi>(operaGxGetRequest, authorization, serverSettings, "gamedev/profile", profileSessionStorage, data => data);
            var refetchables = new IRefetchable[] { allGamesCachedData, allGroupsCachedData, profileCachedData };

            var synchronizer = new GameSynchronizer(refetchables, gameDataStorage, allGroupsCachedData, allGamesCachedData, userInterface);

            var validNamesChecker = new OperaGxValidNamesChecker();
            var sizeAnalyzer = new GameSizeAnalyzer(gameDataStorage, userInterface);
            var postBuildActionsManager = new PostBuildActionsManager(serverSettings, gameDataStorage, synchronizer, uploadRequest, sizeAnalyzer, userInterface, authorization);
            var newGameRegistrator = new NewGameRegistrator(serverSettings, synchronizer, registerGameRequest);
            var automatedProcesses = new AutomatedProcesses(synchronizer, authorization, newGameRegistrator, postBuildActionsManager, gameDataStorage, builder);

            var facade = new GxBusinessLogicFacade(gameDataStorage, allGamesCachedData, allGroupsCachedData, profileCachedData, synchronizer, authorization, validNamesChecker, automatedProcesses, urlOpener);

            return facade;
        }
    }
}

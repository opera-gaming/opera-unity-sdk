using System;
using System.Collections.Generic;

namespace Opera
{
    public class GxBusinessLogicFacade : IGxBusinessLogic
    {
        public event Action Changed;
        public event Action ProcessStarted;
        public event Action<string> ProcessFinishedSuccessfully;
        public event Action<string> ProcessFailed;
        public event Action ProcessSettled;

        public ILocalGameData LocalGameData { get; }
        public bool IsAuthorized => !string.IsNullOrEmpty(operaAuthorization.OAUTH2_access_token);
        public IReadOnlyList<GameDataApi> AllGamesOnCloud => allGamesOnCloudStorage.Data;
        public IReadOnlyList<GroupDataApi> AllGroups => allGroupsStorage.Data;
        public string ProfileName => profileStorage.Data?.username;

        private readonly ICachedData<GameDataApi[]> allGamesOnCloudStorage;
        private readonly ICachedData<GroupDataApi[]> allGroupsStorage;
        private readonly ICachedData<ProfileDataApi> profileStorage;
        private readonly OperaAuthorization operaAuthorization;
        private readonly OperaGxValidNamesChecker validNamesChecker;
        private readonly AutomatedProcesses automatedProcesses;
        private readonly UrlOpener urlOpener;

        public GxBusinessLogicFacade(
            ILocalGameData localGameData,
            ICachedData<GameDataApi[]> allGamesOnCloudStorage,
            ICachedData<GroupDataApi[]> allGroupsStorage,
            ICachedData<ProfileDataApi> profileStorage,
            GameSynchronizer gameSynchronizer,
            OperaAuthorization operaAuthorization,
            OperaGxValidNamesChecker validNamesChecker,
            AutomatedProcesses automatedProcesses,
            UrlOpener urlOpener)
        {
            // Initialize dependencies
            LocalGameData = localGameData ?? throw new ArgumentNullException(nameof(localGameData));
            this.allGamesOnCloudStorage = allGamesOnCloudStorage ?? throw new ArgumentNullException(nameof(allGamesOnCloudStorage));
            this.allGroupsStorage = allGroupsStorage ?? throw new ArgumentNullException(nameof(allGroupsStorage));
            this.profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            this.operaAuthorization = operaAuthorization ?? throw new ArgumentNullException(nameof(operaAuthorization));
            this.validNamesChecker = validNamesChecker ?? throw new ArgumentNullException(nameof(validNamesChecker));
            this.automatedProcesses = automatedProcesses ?? throw new ArgumentNullException(nameof(automatedProcesses));
            this.urlOpener = urlOpener ?? throw new ArgumentNullException(nameof(urlOpener));

            // Subscribe events
            gameSynchronizer.Changed += () => Changed?.Invoke();

            automatedProcesses.ProcessStarted += () => ProcessStarted?.Invoke();
            automatedProcesses.ProcessFinishedSuccessfully += (arg) => ProcessFinishedSuccessfully?.Invoke(arg);
            automatedProcesses.ProcessFailed += (arg) => ProcessFailed?.Invoke(arg);
            automatedProcesses.ProcessSettled += () => ProcessSettled?.Invoke();
        }

        public void Authorize() => operaAuthorization.OAUTH2_GetToken();
        public void Update(bool withSynchronization, bool forceMinimalNextVersion) => automatedProcesses.Update(withSynchronization, forceMinimalNextVersion);
        public void BuildAndUploadGame(string buildDirectory) => automatedProcesses.BuildAndUploadAutomated(buildDirectory);
        public void RegisterGame() => automatedProcesses.RegisterGameAutomated();
        public bool IsGameNameValid() => validNamesChecker.IsValid(LocalGameData.Name);
        public void OpenInGxBrowser(string url) => urlOpener.OpenOperaGXBrowser(url);
    }
}

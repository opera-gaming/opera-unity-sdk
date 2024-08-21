using System;

namespace Opera
{
    public class AutomatedProcesses
    {
        public event Action ProcessStarted;
        public event Action<string> ProcessFinishedSuccessfully;
        public event Action<string> ProcessFailed;
        public event Action ProcessSettled;

        private readonly GameSynchronizer gameSynchronizer;
        private readonly OperaAuthorization operaAuthorization;
        private readonly NewGameRegistrator newGameRegistrator;
        private readonly IBuilder builder;
        private readonly PostBuildActionsManager postBuildActionsManager;
        private readonly ILocalGameData gameData;

        private bool IsAuthorized => !string.IsNullOrEmpty(operaAuthorization.OAUTH2_access_token);

        public AutomatedProcesses(GameSynchronizer gameSynchronizer, OperaAuthorization operaAuthorization, NewGameRegistrator newGameRegistrator, PostBuildActionsManager postBuildActionsManager, ILocalGameData gameData, IBuilder builder)
        {
            this.gameSynchronizer = gameSynchronizer ?? throw new ArgumentNullException(nameof(gameSynchronizer));
            this.operaAuthorization = operaAuthorization ?? throw new ArgumentNullException(nameof(operaAuthorization));
            this.newGameRegistrator = newGameRegistrator ?? throw new ArgumentNullException(nameof(newGameRegistrator));
            this.postBuildActionsManager = postBuildActionsManager ?? throw new ArgumentNullException(nameof(postBuildActionsManager));
            this.gameData = gameData ?? throw new ArgumentNullException(nameof(gameData));
            this.builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public void Update(bool withSynchronization, bool forceMinimalNextVersion)
        {
            if (withSynchronization)
            {
                ActWithEvents(() => Authorize() &&
                                    Synchronize(forceMinimalNextVersion),
                              failureMessage: "Could not update the data. See the console for details.");
            }
            else
            {
                gameSynchronizer.SetDataForNewGame();
            }
        }
        
        public void RegisterGameAutomated()
        {
            ActWithEvents(() => Authorize() &&
                                Synchronize() &&
                                RegisterGame(),
                          successMessage: "Successfully registered the game",
                          failureMessage: "Failed to register the game. See the console for details.");
        }

        public void BuildAndUploadAutomated(string buildDirectory)
        {
            ActWithEvents(() => Authorize() &&
                                Synchronize() &&
                                RegisterGame() &&
                                builder.Build(buildDirectory) &&
                                PostBuildActions(buildDirectory),
                          successMessage: "Successfully uploaded the game",
                          failureMessage: "Failed to upload the game. See the console for details.");
        }

        private bool Authorize()
        {
            if (IsAuthorized)
            {
                return operaAuthorization.OAUTH2_Reauthorise_Act();
            }
            else
            {
                return operaAuthorization.OAUTH2_GetToken();
            }
        }

        private bool Synchronize(bool forceMinimalNextVersion = false)
        {
            return gameSynchronizer.SynchronizeAll(forceMinimalNextVersion);
        }

        private bool RegisterGame()
        {
            var isAlreadyRegistered = !string.IsNullOrEmpty(gameData.Id);

            if (isAlreadyRegistered)
            {
                return true;
            }

            string gameName = gameData.Name;
            string groupId = gameData.Group.studioId;

            return newGameRegistrator.RegisterGame(gameName, groupId);
        }

        private bool PostBuildActions(string buildDirectory)
        {
            var gameId = gameData.Id;
            return postBuildActionsManager.PostBuildActions(gameId, buildDirectory);
        }

        private void ActWithEvents(Func<bool> action, string successMessage = null, string failureMessage = null)
        {
            ProcessStarted?.Invoke();

            var success = action();

            if (success && successMessage != null)
            {
                ProcessFinishedSuccessfully?.Invoke(successMessage);
            }

            if (!success && failureMessage != null)
            {
                ProcessFailed?.Invoke(failureMessage);
            }

            ProcessSettled?.Invoke();
        }
    }
}

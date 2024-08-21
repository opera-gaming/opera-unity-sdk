using System.IO;
using System.IO.Compression;
using System;

namespace Opera
{
    public class PostBuildActionsManager
    {
        private string serverURL => serverSettings.ServerUrl;

        private readonly IServerSettings serverSettings;
        private readonly ILocalGameData gameDataStorage;
        private readonly GameSynchronizer synchronizer;
        private readonly IUploadGameRequest uploadRequest;
        private readonly GameSizeAnalyzer gameSizeAnalyzer;
        private readonly IUserInterface userInterface;
        private readonly OperaAuthorization operaAuthorization;

        private string gameId;
        private string buildDirectory;

        public PostBuildActionsManager(IServerSettings serverSettings, ILocalGameData gameDataStorage, GameSynchronizer synchronizer, IUploadGameRequest uploadRequest, GameSizeAnalyzer gameSizeAnalyzer, IUserInterface userInterface, OperaAuthorization operaAuthorization)
        {
            this.serverSettings = serverSettings ?? throw new ArgumentNullException(nameof(serverSettings));
            this.gameDataStorage = gameDataStorage ?? throw new ArgumentNullException(nameof(gameDataStorage));
            this.synchronizer = synchronizer ?? throw new ArgumentNullException(nameof(synchronizer));
            this.uploadRequest = uploadRequest ?? throw new ArgumentNullException(nameof(uploadRequest));
            this.gameSizeAnalyzer = gameSizeAnalyzer ?? throw new ArgumentNullException(nameof(gameSizeAnalyzer));
            this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
            this.operaAuthorization = operaAuthorization ?? throw new ArgumentNullException(nameof(operaAuthorization));
        }

        public bool PostBuildActions(string gameId, string buildDirectory)
        {
            this.gameId = gameId;
            this.buildDirectory = buildDirectory;

            return CheckBuildSize() &&
                   CompressBuild() &&
                   Upload();
        }

        private bool CheckBuildSize()
        {
            return gameSizeAnalyzer.UploadingIsAllowedFor(buildDirectory);
        }

        private bool CompressBuild()
        {
            try
            {
                userInterface.OnProgressBegin("Compressing the build", "Compressing the build");
                userInterface.DisplayCancelableProgressBar("Compressing the build", "Compressing the build", -1);

                if (File.Exists($"{buildDirectory}.zip"))
                {
                    File.Delete($"{buildDirectory}.zip");
                }

                ZipFile.CreateFromDirectory(buildDirectory, $"{buildDirectory}.zip");

                userInterface.Log("Compressed build successfully");
                userInterface.OnProgressEnd();

                return true;
            }
            catch (Exception e)
            {
                userInterface.LogError($"Unhandled exception on compressing the build: {e}");
                userInterface.OnProgressEnd();

                return false;
            }
        }

        private bool Upload()
        {
            var gameData = uploadRequest.UploadGame(
                operaAuthorization.OAUTH2_access_token,
                gameId: gameId,
                version: gameDataStorage.NextVersion.ToString(),
                serverURL: serverURL,
                filename: $"{buildDirectory}.zip"
            );

            if (gameData != null)
            {
                synchronizer.SetGameData(gameData, refetchAll: false);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

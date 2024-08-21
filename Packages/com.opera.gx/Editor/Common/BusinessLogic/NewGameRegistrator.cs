using System;

namespace Opera
{
    public sealed class NewGameRegistrator
    {
        private string serverURL => serverSettings.ServerUrl;

        private readonly IServerSettings serverSettings;
        private readonly GameSynchronizer synchronizer;
        private readonly RegisterGameRequest registerGameRequest;

        public NewGameRegistrator(IServerSettings serverSettings, GameSynchronizer synchronizer, RegisterGameRequest registerGameRequest)
        {
            this.serverSettings = serverSettings ?? throw new ArgumentNullException(nameof(serverSettings));
            this.synchronizer = synchronizer ?? throw new ArgumentNullException(nameof(synchronizer));
            this.registerGameRequest = registerGameRequest ?? throw new ArgumentNullException(nameof(registerGameRequest));
        }

        public bool RegisterGame(string gameName, string groupId)
        {
            var newGameData = registerGameRequest.GXC_Create(
                serverURL: serverURL,
                gameName: gameName,
                groupId: groupId
            );

            var registeringSuccess = newGameData != null;

            if (registeringSuccess)
            {
                return synchronizer.SetGameData(newGameData, refetchAll: true);
            }
            else
            {
                return false;
            }
        }
    }
}

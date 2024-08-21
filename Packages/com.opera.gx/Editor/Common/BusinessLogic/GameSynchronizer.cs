using System;
using System.Linq;

namespace Opera
{
    public sealed class GameSynchronizer
    {
        public event Action Changed;

        private readonly IRefetchable[] refetchables;
        private readonly ILocalGameData gameDataStorage;
        private readonly ICachedData<GroupDataApi[]> groupsStorage;
        private readonly ICachedData<GameDataApi[]> gamesStorage;
        private readonly IUserInterface userInterface;
        private readonly GroupForNewGameFinder groupForNewGameFinder = new GroupForNewGameFinder();
        private readonly BuildVersionsSynchronizer versionsSynchronizer = new BuildVersionsSynchronizer();

        public GameSynchronizer(IRefetchable[] refetchables, ILocalGameData gameDataStorage, ICachedData<GroupDataApi[]> groupsStorage, ICachedData<GameDataApi[]> gamesStorage, IUserInterface userInterface)
        {
            this.refetchables = refetchables ?? throw new ArgumentNullException(nameof(refetchables));
            this.gameDataStorage = gameDataStorage ?? throw new ArgumentNullException(nameof(gameDataStorage));
            this.groupsStorage = groupsStorage ?? throw new ArgumentNullException(nameof(groupsStorage));
            this.gamesStorage = gamesStorage ?? throw new ArgumentNullException(nameof(gamesStorage));
            this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
        }

        public bool SynchronizeAll(bool forceMinimalNextVersion)
        {
            var success = RefetchAll() &&
                          SynchronizeGameWithGx(forceMinimalNextVersion);

            Changed?.Invoke();

            return success;
        }

        private bool RefetchAll()
        {
            var refetchSuccess = true;

            foreach (var refetchable in refetchables)
                refetchSuccess &= refetchable.RefetchData();

            return refetchSuccess;
        }

        private bool SynchronizeGameWithGx(bool forceMinimalNextVersion)
        {
            var localGameId = gameDataStorage.Id;

            if (string.IsNullOrEmpty(localGameId))
            {
                return SynchronizeNewGame(forceMinimalNextVersion);
            }
            else
            {
                return SynchronizeExistingGame(localGameId, forceMinimalNextVersion);
            }
        }
        
        private bool SynchronizeExistingGame(string gameId, bool forceMinimalNextVersion)
        {
            var games = gamesStorage.Data;
            var cloudGameData = games.FirstOrDefault(game => game.gameId == gameId);

            if (cloudGameData == null)
            {
                userInterface.LogWarning("Synchronization error: Game data has not been found on the cloud. You may register it as a new game.");
                SynchronizeNewGame(forceMinimalNextVersion);
                return false;
            }

            SetGameData(cloudGameData, refetchAll: false, forceMinimalNextVersion);
            return true;
        }

        private bool SynchronizeNewGame(bool forceMinimalNextVersion)
        {
            var (success, group) = groupForNewGameFinder.FindGroup(gameDataStorage.Group, groupsStorage.Data);
            if (!success)
            {
                userInterface.Log("Could not find the same group when updating data for the new game.");
            }

            BuildVersion nextVersion = CurrentNextVersion();

            gameDataStorage.Set(
                name: gameDataStorage.Name,
                group: group,
                version: new BuildVersion(),
                nextVersion: versionsSynchronizer.FindNewNextVersion("", new BuildVersion(), nextVersion, forceMinimalNextVersion)
            );

            return success;
        }

        public void SetDataForNewGame()
        {
            gameDataStorage.Set(
                name: gameDataStorage.Name,
                group: gameDataStorage.Group,
                version: new BuildVersion(),
                nextVersion: CurrentNextVersion()
            );
        }

        public bool SetGameData(GameDataApi cloudGameData, bool refetchAll, bool forceMinimalNextVersion = false)
        {
            var refetchSuccess = true;

            if (refetchAll)
            {
                refetchSuccess = RefetchAll();
            }

            var version = new BuildVersion(cloudGameData.version ?? "");
            BuildVersion nextVersion = CurrentNextVersion();

            gameDataStorage.Set(
                name: cloudGameData.title,
                editUrl: cloudGameData.editUrl,
                group: cloudGameData.studio,
                id: cloudGameData.gameId,
                internalShareUrl: cloudGameData.internalShareUrl,
                publicShareUrl: cloudGameData.publicShareUrl,
                version: version,
                nextVersion: versionsSynchronizer.FindNewNextVersion(cloudGameData.gameId, version, nextVersion, forceMinimalNextVersion)
            );

            Changed?.Invoke();

            return refetchSuccess;
        }

        private BuildVersion CurrentNextVersion()
        {
            return gameDataStorage.NextVersion ?? new BuildVersion();
        }
    }
}

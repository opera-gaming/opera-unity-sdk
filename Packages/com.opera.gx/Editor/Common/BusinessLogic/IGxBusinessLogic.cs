using System;
using System.Collections.Generic;

namespace Opera
{
    public interface IGxBusinessLogic
    {
        event Action Changed;
        event Action ProcessStarted;
        event Action<string> ProcessFinishedSuccessfully;
        event Action<string> ProcessFailed;
        event Action ProcessSettled;

        /// <summary>
        /// Game data currently saved locally on the disc
        /// </summary>
        ILocalGameData LocalGameData { get; }

        bool IsAuthorized { get; }

        /// <summary>
        /// All games which belong to the user. If the user is not authorized this list is empty.
        /// </summary>
        IReadOnlyList<GameDataApi> AllGamesOnCloud { get; }

        /// <summary>
        /// All groups where the user is a member. If the user is not authorized this list is empty.
        /// </summary>
        IReadOnlyList<GroupDataApi> AllGroups { get; }

        string ProfileName { get; }

        void Authorize();
        void Update(bool withSynchronization, bool forceMinimalNextVersion);

        void RegisterGame();
        void BuildAndUploadGame(string buildDirectory);
        bool IsGameNameValid();

        void OpenInGxBrowser(string url);
    }
}

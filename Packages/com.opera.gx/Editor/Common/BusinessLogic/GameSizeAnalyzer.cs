using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Opera
{
    public class GameSizeAnalyzer
    {
        public static readonly long bytesInMegabytes = 1024 * 1024;

        private readonly ILocalGameData gameDataStorage;
        private readonly IUserInterface userInterface;

        public GameSizeAnalyzer(ILocalGameData gameDataStorage, IUserInterface userInterface)
        {
            this.gameDataStorage = gameDataStorage ?? throw new ArgumentNullException(nameof(gameDataStorage));
            this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
        }

        public bool UploadingIsAllowedFor(string pathToFolder)
        {
            var actualSizeBytes = GetFilesSizeBytes(pathToFolder);
            long gameMaxSizeMegaBytes = gameDataStorage.Group.gameMaxSize;
            long gameMaxSizeBytes = gameMaxSizeMegaBytes * bytesInMegabytes;

            if (actualSizeBytes <= gameMaxSizeBytes)
                return true;

            userInterface.Log(string.Format("Cannot upload the game to GX.Games. Project Size (unpacked): {0:0.##} MB. Maximum allowed size on GX.Games: {1:0.##} MB", actualSizeBytes / (float)bytesInMegabytes, gameMaxSizeMegaBytes));
            return false;
        }

        private long GetFilesSizeBytes(string folderPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            return dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
        }
    }
}

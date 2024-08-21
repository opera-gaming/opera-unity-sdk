namespace Opera
{
    public class BuildVersionsSynchronizer
    {
        public BuildVersion FindNewNextVersion(string gameId, BuildVersion currentVersionObj, BuildVersion nextVersionObj, bool forceMinimalNextVersion)
        {
            if (forceMinimalNextVersion)
            {
                return IsNewProject(gameId) ? new BuildVersion() : IncrementVersion(currentVersionObj);
            }

            if (nextVersionObj > currentVersionObj) return nextVersionObj;

            return IncrementVersion(currentVersionObj);
        }

        private BuildVersion IncrementVersion(BuildVersion currentVersionObj)
        {
            var newNextVersion = new BuildVersion(currentVersionObj);
            newNextVersion.revision++;
            return newNextVersion;
        }

        private bool IsNewProject(string gameId) => string.IsNullOrEmpty(gameId);
    }
}

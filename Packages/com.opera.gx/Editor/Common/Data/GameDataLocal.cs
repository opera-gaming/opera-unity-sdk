namespace Opera
{
    [System.Serializable]
    public sealed class GameDataLocal
    {
        public string name;
        public string editUrl;
        public GroupDataApi group;
        public string id;
        public string internalShareUrl;
        public string projectId;
        public string publicShareUrl;
        public BuildVersion version;
        public BuildVersion nextVersion;
    }
}

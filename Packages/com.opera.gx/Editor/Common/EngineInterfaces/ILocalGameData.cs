namespace Opera
{
    public interface ILocalGameData
    {
        string Name { get; set; }
        string EditUrl { get; set; }
        GroupDataApi Group { get;set; }
        string Id {  get; set; }
        string InternalShareUrl {  get; set; }
        string PublicShareUrl {  get; set; }
        BuildVersion Version {  get; set; }
        BuildVersion NextVersion {  get; set; }

        void Set(
            string name = "",
            string editUrl = "",
            GroupDataApi group = null,
            string id = "",
            string internalShareUrl = "",
            string publicShareUrl = "",
            BuildVersion version = null,
            BuildVersion nextVersion = null);
    }
}

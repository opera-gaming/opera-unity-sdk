using System;

namespace Opera
{
    public interface ISessionStorage
    {
        string OAUTH2_access_token { get; set; }
        string OAUTH2_refresh_token { get; set; }
        int OAUTH2_expires_in { get; set; }
        DateTime Expiry { get; set; }

        GameDataApi[] GamesData { get; set; }
        GroupDataApi[] GroupsData { get; set; }
        ProfileDataApi ProfileData { get; set; }
    }

    public interface ISessionProperty<TData>
    {
        TData Value { get; set; }
    }

    public sealed class SessionGamesData : ISessionProperty<GameDataApi[]>
    {
        private readonly ISessionStorage sessionStorage;

        public SessionGamesData(ISessionStorage sessionStorage)
        {
            this.sessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
        }

        public GameDataApi[] Value
        {
            get => sessionStorage.GamesData;
            set => sessionStorage.GamesData = value;
        }
    }

    public sealed class SessionGroupsData : ISessionProperty<GroupDataApi[]>
    {
        private readonly ISessionStorage sessionStorage;

        public SessionGroupsData(ISessionStorage sessionStorage)
        {
            this.sessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
        }

        public GroupDataApi[] Value
        {
            get => sessionStorage.GroupsData;
            set => sessionStorage.GroupsData = value;
        }
    }

    public sealed class SessionProfileData : ISessionProperty<ProfileDataApi>
    {
        private readonly ISessionStorage sessionStorage;

        public SessionProfileData(ISessionStorage sessionStorage)
        {
            this.sessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
        }

        public ProfileDataApi Value
        {
            get => sessionStorage.ProfileData;
            set => sessionStorage.ProfileData = value;
        }
    }
}

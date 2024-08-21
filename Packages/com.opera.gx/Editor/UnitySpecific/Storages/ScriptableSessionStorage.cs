using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Opera
{
    public class ScriptableSessionStorage : ScriptableSingleton<ScriptableSessionStorage>, ISessionStorage
    {
        public string OAUTH2_access_token { get; set; }
        public string OAUTH2_refresh_token { get; set; }
        public int OAUTH2_expires_in { get; set; }
        public DateTime Expiry { get; set; }

        public GameDataApi[] GamesData { get; set; }
        public GroupDataApi[] GroupsData { get; set; }
        public ProfileDataApi ProfileData { get; set; }
    }
}

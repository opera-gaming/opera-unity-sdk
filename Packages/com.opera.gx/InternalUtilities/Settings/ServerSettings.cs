using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opera
{
    // Uncomment for debugging or creating files with custom settings
    //[CreateAssetMenu(fileName = "ServerSettings", menuName = "ServerSettings")]
    public sealed class ServerSettings : ScriptableObject, IServerSettings
    {
        public string ServerUrl => serverUrl;
        public string OauthServer => oauthServer;

        public string serverUrl;
        public string oauthServer;
    }
}

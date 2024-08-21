using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opera
{
    public static class ServerSettingsFinder
    {
        public static IServerSettings FindServerSettings()
        {
            var settingsAsset = Resources.Load<ServerSettings>("ServerSettings");

            if (settingsAsset == null)
            {
                return new DefaultServerSettings();
            }
            else
            {
                Debug.Log("GX.Games: Using custom server settings. If you are in Unity Editor, click this message to locate the config file in the project.", settingsAsset);
                return settingsAsset;
            }
        }
    }
}

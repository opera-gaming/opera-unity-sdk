using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Opera
{
    /// <summary>
    /// This is a class which provides access to the business logic.
    /// It also contains factory methods which create:
    /// - Unity-specific dependencies;
    /// - the business logic class.
    /// </summary>
    public class GxBusinessLogicSingleton
    {
        // May be extracted to settings if we need it, so that the user would be able to change it.
        public const string BUILD_DIRECTORY = "GxGamesBuild";

        private const string REDIRECT = "http://localhost:8890/";
        private const string CLIENT_ID = "gg-unity-plugin";
        private const string ENGINE_ALIAS = "unity";

        private static IGxBusinessLogic instance;
        public static IGxBusinessLogic Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = CreateInstance();

                EditorApplication.projectChanged += () =>
                {
                    // TODO: We may check if the server settings file has been changed. If it did,
                    // we should create the new instance.

                    if (!SettingsAssetExists())
                    {
                        Debug.LogWarning("Settings asset has disappeared! Restarting Gx.Games plugin");
                        instance = CreateInstance();
                    }
                };
                return instance;
            }
        }

        /// <summary>
        /// Creating the business logic instance with all the Unity-specific dependencies
        /// </summary>
        /// <returns></returns>
        private static IGxBusinessLogic CreateInstance()
        {
            var userInterface = new UserInterfaceAdapter();
            var builder = new UnityBuilder();
            var jsonUtility = new JsonUtilityAdapter();
            var gameDataStorage = FindGameDataStorage();
            var serverSettings = ServerSettingsFinder.FindServerSettings();
            var sessionStorage = ScriptableSessionStorage.instance;
            var httpRequest = new StandardHttpRequestWrapper();
            var operaGxGetRequest = new OperaGxGetRequest(jsonUtility, userInterface);
            var uploadRequest = new UploadGameRequest(jsonUtility, userInterface);
            var applicationFocuser = CreateApplicationFocuser();

            var _instance = GxBusinessLogicFactory.CreateBusinessLogic(
                jsonUtility,
                userInterface,
                builder,
                gameDataStorage,
                sessionStorage,
                serverSettings,
                httpRequest,
                operaGxGetRequest,
                uploadRequest,
                applicationFocuser,
                REDIRECT,
                CLIENT_ID,
                ENGINE_ALIAS);
            
            return _instance;
        }

        private static IApplicationFocusStrategy CreateApplicationFocuser()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))              
                return new ApplicationFocusWindows();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return new ApplicationFocusMac();

            return new ApplicationFocusEmpty();
        }

        private static bool SettingsAssetExists() => AssetDatabase.LoadAssetAtPath<GameDataGx>("Assets/OperaSDK/GameDataGx.asset") != null;

        private static GameDataGx FindGameDataStorage()
        {
            var settingsAsset = AssetDatabase.LoadAssetAtPath<GameDataGx>("Assets/OperaSDK/GameDataGx.asset");

            if (settingsAsset == null)
            {
                settingsAsset = ScriptableObject.CreateInstance<GameDataGx>();
                if (!Directory.Exists("Assets/OperaSDK"))
                {
                    AssetDatabase.CreateFolder("Assets", "OperaSDK");
                }
                AssetDatabase.CreateAsset(settingsAsset, "Assets/OperaSDK/GameDataGx.asset");

                // Assigning default value after creating the asset to avoid serialization errors
                settingsAsset.Name = GetDefaultGameName();
                
                AssetDatabase.SaveAssets();
            }

            return settingsAsset;
        }

        private static string GetDefaultGameName() => Application.productName;
    }
}

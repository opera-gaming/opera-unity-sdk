using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Opera
{
    public class WebGlTemlpateFilesManager : AssetPostprocessor
    {
        public static bool TemplateExists
        {
            get
            {
                TryUpdateCachedSettings();
                return templateExists;
            }
        }

        public static bool AreTemplatesSameInAssetsAndPackagesFolder
        {
            get
            {
                TryUpdateCachedSettings();
                return areTemplatesSameInAssetsAndPackagesFolder;
            }
        }

        private const string PATH_TO_WEBGL_TEMPLATE_IN_ASSETS_FOLDER = "Assets/WebGLTemplates/GxGames";
        private const string PATH_TO_WEBGL_TEMPLATE_IN_PACKAGES_FOLDER = "Packages/com.opera.gx/WebGLTemplates/GxGames";

        private static bool isFileFlagsOutdated = true;
        private static bool isFileWatchingOn;
        private static bool templateExists;
        private static bool areTemplatesSameInAssetsAndPackagesFolder;

        private static readonly DirectoriesComparer directoriesComparer = new DirectoriesComparer(new FilesContentComparer());
        private static FileSystemWatcher fileSystemWatcher;

        /// <summary>
        /// It looks like it's not possible to keep the custom template in "Packages" folder. They 
        /// should be stored only in "Assets/WebGLTemplates".
        /// That's why we need this function.
        /// </summary>
        public static void CopyTemplateToAssetsFolder()
        {
            StopWatchingProjectChanges();

            AssetDatabase.DeleteAsset(PATH_TO_WEBGL_TEMPLATE_IN_ASSETS_FOLDER);

            if (!AssetDatabase.IsValidFolder("Assets/WebGLTemplates"))
                AssetDatabase.CreateFolder("Assets", "WebGLTemplates");
                        
            AssetDatabase.CopyAsset(PATH_TO_WEBGL_TEMPLATE_IN_PACKAGES_FOLDER, PATH_TO_WEBGL_TEMPLATE_IN_ASSETS_FOLDER);

            StartWatchingProjectChanges();
        }

        /// <summary>
        /// A built-in Unity Message which is sent to the classes inherited from AssetPostprocessor
        /// </summary>
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            // Copy template on importing this package.
            if (IsImportedFilesBelongToThisPackage(importedAssets))
            {
                if (!IsWebTemplateExistsInAssetsFolder())
                {
                    CopyTemplateToAssetsFolder();
                    Debug.Log("Copied a web template for GX.Games into \"Assets/WebGLTemplates\".");
                }
            }

            // Make cache outdated.
            if (isFileWatchingOn)
            {
                isFileFlagsOutdated = true;
            }
        }

        public static void StartWatchingProjectChanges()
        {
            isFileFlagsOutdated = true;
            isFileWatchingOn = true;
        }

        public static void StopWatchingProjectChanges()
        {
            isFileWatchingOn = false;
        }

        // This function should run in the main thread because of the usage of `AssetDatabase` class.
        // THe operations related to the file system are cached - sometimes
        // they become too heavy to repeat them on OnGUI() stage.
        private static void TryUpdateCachedSettings()
        {
            if (!isFileFlagsOutdated) return;

            try
            {
                templateExists = IsWebTemplateExistsInAssetsFolder();

                areTemplatesSameInAssetsAndPackagesFolder = directoriesComparer.AreDirectoriesEqual(
                    PATH_TO_WEBGL_TEMPLATE_IN_ASSETS_FOLDER,
                    PATH_TO_WEBGL_TEMPLATE_IN_PACKAGES_FOLDER);
            }
            catch (System.Exception exception)
            {
                Debug.LogError("Uncaught exception on reading web template files: " + exception);
            }
            finally
            {
                isFileFlagsOutdated = false;
            }
        }

        private static bool IsWebTemplateExistsInAssetsFolder()
        {
            return AssetDatabase.IsValidFolder(PATH_TO_WEBGL_TEMPLATE_IN_ASSETS_FOLDER);
        }

        // This check is necessary to detect whether the package has been just imported.
        // It is supposed to run only after adding/updating the package's files.
        private static bool IsImportedFilesBelongToThisPackage(string[] importedAssets)
        {
            return importedAssets.Any(assetPath => assetPath.StartsWith("Packages/com.opera.gx"));
        }
    }
}


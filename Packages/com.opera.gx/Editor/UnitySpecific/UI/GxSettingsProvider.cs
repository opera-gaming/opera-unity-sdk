using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GUILayout;
using static Opera.GUILayoutWrappers;

namespace Opera
{
    public class GxSettingsProvider : SettingsProvider
    {
        private IGxBusinessLogic businessLogic => GxBusinessLogicSingleton.Instance;

        public GxSettingsProvider() : base("Project/GX.Games", SettingsScope.Project) { }

        [SettingsProvider]
        public static SettingsProvider CreateGxSettingsProvider()
        {
            return new GxSettingsProvider();
        }

        private int selectedGameIndex = 0;
        private int selectedGroupIndex = 0;
        private bool isGameNameValid = true;
        private GameOption[] gameOptions = new GameOption[0];
        private GroupDataApi[] groups = new GroupDataApi[0];

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            UpdateUiAccordingToBusinessLogic();

            businessLogic.Changed += UpdateUiAccordingToBusinessLogic;
            businessLogic.ProcessFinishedSuccessfully += ShowMessage;
            businessLogic.ProcessFailed += ShowMessage;

            WebGlTemlpateFilesManager.StartWatchingProjectChanges();
        }

        public override void OnDeactivate()
        {
            businessLogic.Changed -= UpdateUiAccordingToBusinessLogic;
            businessLogic.ProcessFinishedSuccessfully -= ShowMessage;
            businessLogic.ProcessFailed -= ShowMessage;

            WebGlTemlpateFilesManager.StopWatchingProjectChanges();
        }

        private void UpdateUiAccordingToBusinessLogic()
        {
            // Set new game options list
            gameOptions = CreateGameOptions();
            selectedGameIndex = gameOptions.ToList().FindIndex(item => item.gameId == (businessLogic.LocalGameData.Id ?? ""));

            // Set group options list
            groups = CreateGroupOptions();
            selectedGroupIndex = groups.ToList().FindIndex(item => item.studioId == (businessLogic.LocalGameData?.Group?.studioId ?? ""));
        }

        private void ShowMessage(string message) => EditorUtility.DisplayDialog("Gx.Games", message, "OK");

        public override void OnGUI(string searchContext)
        {
            // Callbacks
            void onAuthorizeButtonClick()
            {
                businessLogic.Update(withSynchronization: true, forceMinimalNextVersion: false);
            };

            void onSelectedGameChanged(int newSelectedIndex)
            {
                selectedGameIndex = newSelectedIndex;
                
                string newGameId = gameOptions[selectedGameIndex].gameId;
                businessLogic.LocalGameData.Id = newGameId;

                var newGameIsNew = string.IsNullOrEmpty(newGameId);
                var skipSynchronization = !businessLogic.IsAuthorized && newGameIsNew;
                businessLogic.Update(withSynchronization: !skipSynchronization, forceMinimalNextVersion: true);
            }

            void onSelectedGroupChanged(int newSelectedIndex)
            {
                selectedGroupIndex = newSelectedIndex;
                businessLogic.LocalGameData.Group = groups[selectedGroupIndex];
            }

            var isNewGame = gameOptions[selectedGameIndex].IsNewGame;
            isGameNameValid = businessLogic.IsGameNameValid();
            
            // Render
            EditorGUIUtility.labelWidth = 240;

            Label("Recommended settings for GX.Games", EditorStyles.boldLabel);
            RecommendedSettingsWarning();
            Space(12);

            Label("Authorization on GX.Games", EditorStyles.boldLabel);
            Horizontal(() =>
            {
                W(() => Button(EditorGUIUtility.IconContent("TreeEditor.Refresh"), Width(24), Height(24)), iconSize: 16, onClick: onAuthorizeButtonClick);
                Label(businessLogic.IsAuthorized ? $"You are authorized as {businessLogic.ProfileName}" : "You are not authorzied", style: Styles.AuthorizedLabelStyle, Height(24));
            });
            Space(12);

            Label("Product information", EditorStyles.boldLabel);
            Popup("Select game", selectedGameIndex, gameOptions.Select(opt => opt.optionName).ToArray(), onChange: onSelectedGameChanged);
            W(() => businessLogic.LocalGameData.Name = EditorGUILayout.TextField("Game Name", businessLogic.LocalGameData.Name), disabled: !isNewGame);
            if (!isGameNameValid) EditorGUILayout.HelpBox("The name is empty or contains invalid characters.", MessageType.Error);
            Version("Version", disabled: true, businessLogic.LocalGameData.Version);
            Version("Next version", disabled: false, businessLogic.LocalGameData.NextVersion);
            Popup("Select group (for new games only)", selectedGroupIndex, groups.Select(g => g.name).ToArray(), disabled: !isNewGame, onChange: onSelectedGroupChanged);
            W(() => Button("Register on GX.Games"), disabled: !isNewGame || !isGameNameValid, onClick: businessLogic.RegisterGame);
            W(() => Button("Build && Upload"), disabled: !isGameNameValid, onClick: () => businessLogic.BuildAndUploadGame(GxBusinessLogicSingleton.BUILD_DIRECTORY));
            Space(12);

            LinkButton("Edit Game on Opera", businessLogic.LocalGameData.EditUrl);
            LinkButton("Internal Share URL", businessLogic.LocalGameData.InternalShareUrl);
            LinkButton("Public Share URL", businessLogic.LocalGameData.PublicShareUrl);

            // Respond to changes
            if (GUI.changed)
            {
            }
        }

        private void RecommendedSettingsWarning()
        {
            bool areSettingsRecommended = PlayerSettingsController.AreSettingsRecommended();
            bool webTemplateExists = WebGlTemlpateFilesManager.TemplateExists;
            bool areTemplatesSameInAssetsAndPackagesFolder = WebGlTemlpateFilesManager.AreTemplatesSameInAssetsAndPackagesFolder;

            if (areSettingsRecommended &&
                webTemplateExists &&
                areTemplatesSameInAssetsAndPackagesFolder)
            {
                EditorGUILayout.HelpBox("Your project settings are compatible with GX.Games.", MessageType.None);
            }

            if (!webTemplateExists)
            {
                EditorGUILayout.HelpBox("Could not find the custom WebGL template for GX.Games in the folder \"Assets/WebGLTemplates\". Press the button below to copy it from Packages folder.", MessageType.Warning);
            }

            if (webTemplateExists && !areTemplatesSameInAssetsAndPackagesFolder)
            {
                EditorGUILayout.HelpBox("WebGL template for GX.Games in the folder \"Assets/WebGLTemplates\" is changed or outdated. Press the button below to copy it from Packages folder.", MessageType.Warning);
            }

            if (!webTemplateExists || !areTemplatesSameInAssetsAndPackagesFolder)
            {
                W(() => Button("Copy the WebGL template to Assets"), onClick: WebGlTemlpateFilesManager.CopyTemplateToAssetsFolder);
            }

            if (!areSettingsRecommended)
            {
                var templateMessage = !PlayerSettingsController.IsTemplateRecommended() ? "WebGL Template" : null;
                var compressionMessage = !PlayerSettingsController.IsCompressionRecommended() ? "Compression Format" : null;

                var incorrectProperties = string.Join(",", new[] { templateMessage, compressionMessage }.Where(msg => msg != null));

                var warningMessage = $"It is recommended to change some projects settings to make it compatible with GX.Games. " +
                    $"Please check the following settings: {incorrectProperties}. " +
                    $"Or you may just press the button to set recommended settings.";
                EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
                W(() => Button("Set Recommended Settings"), onClick: PlayerSettingsController.SetRecommendedSettings);
            }
        }

        private void Version(string label, bool disabled, BuildVersion version)
        {
            W(disabled: disabled, renderFunc: () => Horizontal(() =>
            {
                EditorGUILayout.PrefixLabel(label);
                IntField(version.major, Styles.BuildVersionStyle, onChange: (value) => version.major = value);
                IntField(version.minor, Styles.BuildVersionStyle, onChange: (value) => version.minor = value);
                IntField(version.build, Styles.BuildVersionStyle, onChange: (value) => version.build = value);
                IntField(version.revision, Styles.BuildVersionStyle, onChange: (value) => version.revision = value);
            }));
        }

        private void LinkButton(string text, string url)
        {
            var d = string.IsNullOrEmpty(url);
            W(() => Button(text), disabled: string.IsNullOrEmpty(url), onClick: () => businessLogic.OpenInGxBrowser(url));
        }

        private GameOption[] CreateGameOptions()
        {
            var gameOptions = businessLogic.AllGamesOnCloud?.Select(g => new GameOption
            {
                gameId = g.gameId,
                optionName = g.title,
            }) ?? new GameOption[0];

            var newOptions = new[] { GameOption.newGameOption }.Concat(gameOptions).ToList();

            var currentLocalGame = businessLogic.LocalGameData;
            if (!newOptions.Any(option => option.gameId == (businessLogic.LocalGameData.Id ?? "")))
            {
                newOptions.Add(new GameOption
                {
                    gameId = currentLocalGame.Id,
                    optionName = currentLocalGame.Name,
                });
            }

            return newOptions.ToArray();
        }

        private GroupDataApi[] CreateGroupOptions()
        {
            return businessLogic.AllGroups?.ToArray() ?? new[] { businessLogic.LocalGameData.Group };
        }
    }

    public static class Styles
    {
        public static readonly GUIStyle AuthorizedLabelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
        };

        public static readonly GUIStyle BuildVersionStyle = new GUIStyle(GUI.skin.textField)
        {
            alignment = TextAnchor.MiddleRight,
        };
    }
}

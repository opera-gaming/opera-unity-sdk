using UnityEditor;
using UnityEngine;

namespace Opera
{
    // Uncomment for testing purposes:
    //[CreateAssetMenu(fileName = "GameDataGx", menuName = "GameDataGx")]
    public class GameDataGx : ScriptableObject, ILocalGameData
    {
        public GameDataLocal gameData;

        public string Name { get => gameData.name; set { gameData.name = value; SetThisDirty(); } }
        public string EditUrl { get => gameData.editUrl; set { gameData.editUrl = value; SetThisDirty(); } }
        public GroupDataApi Group { get => gameData.group; set { gameData.group = value; SetThisDirty(); } }
        public string Id { get => gameData.id; set { gameData.id = value; SetThisDirty(); } }
        public string InternalShareUrl { get => gameData.internalShareUrl; set { gameData.internalShareUrl = value; SetThisDirty(); } }
        public string PublicShareUrl { get => gameData.publicShareUrl; set { gameData.publicShareUrl = value; SetThisDirty(); } }
        public BuildVersion Version { get => gameData.version; set { gameData.version = value; SetThisDirty(); } }
        public BuildVersion NextVersion { get => gameData.nextVersion; set { gameData.nextVersion = value; SetThisDirty(); } }

        public void Set(
            string name = "", 
            string editUrl = "", 
            GroupDataApi group = null, 
            string id = "", 
            string internalShareUrl = "", 
            string publicShareUrl = "", 
            BuildVersion version = null, 
            BuildVersion nextVersion = null)
        {
            gameData.name = name;
            gameData.editUrl = editUrl;
            gameData.group = group;
            gameData.id = id;
            gameData.internalShareUrl = internalShareUrl;
            gameData.publicShareUrl= publicShareUrl;
            gameData.version = version;
            gameData.nextVersion = nextVersion;
        }

        private void SetThisDirty() => EditorUtility.SetDirty(this);
    }
}

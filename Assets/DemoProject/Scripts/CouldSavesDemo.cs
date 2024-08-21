using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Opera
{
    public sealed class CouldSavesDemo : MonoBehaviour
    {
        public InputField scoreField;
        public Button saveButton;

        private void Awake()
        {
            saveButton.onClick.AddListener(() => Save(scoreField.text));
        }

        private void Start()
        {
            scoreField.text = Load() ?? "0";
        }

        private void Save(string score)
        {
            var directory = GxGames.GenerateDataPath() ?? Application.persistentDataPath;

            var filePath = Path.Combine(directory, "save");
            Debug.Log("Saving into: " + filePath);

            File.WriteAllText(filePath, score);

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                SyncFiles();
            }
        }

        private string Load()
        {
            var directory = GxGames.GenerateDataPath() ?? Application.persistentDataPath;

            var filePath = Path.Combine(directory, "save");
            Debug.Log("Loading from: " + filePath);

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            return null;
        }

        [DllImport("__Internal")]
        private static extern void SyncFiles();
    }
}

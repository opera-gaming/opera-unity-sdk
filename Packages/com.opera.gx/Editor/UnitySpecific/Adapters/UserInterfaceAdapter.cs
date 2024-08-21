using UnityEditor;
using UnityEngine;

namespace Opera
{
    public sealed class UserInterfaceAdapter : IUserInterface
    {
        public void Log(string message) => Debug.Log(message);
        public void LogWarning(string message) => Debug.LogWarning(message);
        public void LogError(string message) => Debug.LogError(message);

        public void OnProgressBegin(string title, string info) { }
        public bool DisplayCancelableProgressBar(string title, string info, float progressValue) => EditorUtility.DisplayCancelableProgressBar(title, info, progressValue);
        public void OnProgressEnd() => EditorUtility.ClearProgressBar();
    }
}

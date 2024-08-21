using System;
using UnityEditor;
using UnityEngine;

namespace Opera
{
    public static class GUILayoutWrappers
    {
        // Wrapper for non-interactable components
        public static void W(Action renderFunc, bool disabled = false)
        {
            var currentGuiEnabled = GUI.enabled;
            GUI.enabled = !disabled;

            renderFunc?.Invoke();

            GUI.enabled = currentGuiEnabled;
        }

        // Wrapper with more convenient ways to disable buttons and to pass onClick handlers
        public static void W(Func<bool> renderButtonFunc, bool disabled = false, float iconSize = default, Action onClick = null)
        {
            if (renderButtonFunc == null) return;

            var currentGuiEnabled = GUI.enabled;
            GUI.enabled = !disabled;

            EditorGUIUtility.SetIconSize(new Vector2(iconSize, iconSize));

            if (renderButtonFunc.Invoke())
            {
                onClick?.Invoke();
                GUIUtility.ExitGUI();
            }

            EditorGUIUtility.SetIconSize(Vector2.zero);

            GUI.enabled = currentGuiEnabled;
        }

        // Wrapper for text input field
        public static void W(Func<string> renderFieldFunc, bool disabled = false, Action<string> setText = null)
        {
            if (renderFieldFunc == null) return;

            var currentGuiEnabled = GUI.enabled;
            GUI.enabled = !disabled;

            var newText = renderFieldFunc();
            setText?.Invoke(newText);

            GUI.enabled = currentGuiEnabled;
        }

        // A wrapper for horizontal container which forces to always call "Begin" and "End" functions.
        public static void Horizontal(Action children = null)
        {
            GUILayout.BeginHorizontal();
            children?.Invoke();
            GUILayout.EndHorizontal();
        }

        public static void Popup(string label, int selectedIndex, string[] displayedOptions, bool disabled = false, Action<int> onChange = null)
        {
            var currentGuiEnabled = GUI.enabled;
            GUI.enabled = !disabled;

            var newSelectedIndex = EditorGUILayout.Popup(label, selectedIndex, displayedOptions);

            GUI.enabled = currentGuiEnabled;

            if (selectedIndex != newSelectedIndex)
            {
                onChange?.Invoke(newSelectedIndex);
            }
        }

        public static void IntField(int value, GUIStyle style, Action<int> onChange = null)
        {
            var newValueString = EditorGUILayout.TextField(value.ToString(), style);
            int.TryParse(newValueString, out var newValue);

            if (value != newValue)
            {
                onChange?.Invoke(newValue);
            }
        }
    }
}

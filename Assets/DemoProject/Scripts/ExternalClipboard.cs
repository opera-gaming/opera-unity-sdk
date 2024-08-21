using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ExternalClipboard : MonoBehaviour
{
    public InputField inputfield;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            CopyPasteReader(gameObject.name, "Paste");
        }
        else
        {
            Debug.LogWarning("This button is for WebGL. Use Ctrl+V on other platforms");
        }
    }

    private void Paste(string pasteValue)
    {
        inputfield.text = pasteValue;
    }

    [DllImport("__Internal")]
    public static extern void CopyPasteReader(string gObj, string vName);
}

using System;
using System.Runtime.InteropServices;

namespace Opera
{
    public sealed class ApplicationFocusWindows : IApplicationFocusStrategy
    {
        private IntPtr windowHandle;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        public void OnBeforeUnfocusing()
        {
            // storing the window handle before unfocusing is more reliable than using Process.GetCurrentProcess().
            // Process.GetCurrentProcess() may return a new handle on every call which would not work at least with Unity.
            windowHandle = GetActiveWindow();
            return;
        }

        public void FocusEditorWindow()
        {
            SetForegroundWindow(windowHandle);
        }
    }
}

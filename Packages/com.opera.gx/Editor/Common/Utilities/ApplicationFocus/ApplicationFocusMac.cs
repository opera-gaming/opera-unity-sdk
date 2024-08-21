using System.Diagnostics;

namespace Opera
{
    public sealed class ApplicationFocusMac : IApplicationFocusStrategy
    {
        public void OnBeforeUnfocusing() { }

        public void FocusEditorWindow()
        {
            var processInfo = Process.GetCurrentProcess();
            Process.Start(new ProcessStartInfo("osascript", string.Format("-e \'tell application \"{0}\" to activate\'", processInfo.ProcessName)));

            return;
        }
    }
}

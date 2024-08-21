namespace Opera
{
    public sealed class ApplicationFocusEmpty : IApplicationFocusStrategy
    {
        public void OnBeforeUnfocusing() { }
        public void FocusEditorWindow() { }
    }
}

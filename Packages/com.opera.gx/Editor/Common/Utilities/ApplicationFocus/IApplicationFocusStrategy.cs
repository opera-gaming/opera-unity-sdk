namespace Opera
{
    public interface IApplicationFocusStrategy
    {
        void OnBeforeUnfocusing();
        void FocusEditorWindow();
    }
}

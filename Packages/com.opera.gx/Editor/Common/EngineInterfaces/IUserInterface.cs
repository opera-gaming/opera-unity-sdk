namespace Opera
{
    public interface IUserInterface
    {
        void Log(string message);
        void LogError(string message);
        void LogWarning(string message);

        void OnProgressBegin(string title, string info);
        bool DisplayCancelableProgressBar(string title, string info, float progressValue);
        void OnProgressEnd();
    }
}

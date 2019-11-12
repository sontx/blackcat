namespace Blackcat.UI
{
    public interface IMessageBoxHost
    {
        void ShowError(string msg);

        void ShowWarning(string msg);

        void ShowInfo(string msg);

        bool ShowQuestion(string msg, bool serious = false);
    }
}
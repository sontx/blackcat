using System;

namespace Blackcat.OS
{
    public interface IReportWindow
    {
        void Initialize(Exception exception, string productName, string devMail);

        void Show();
    }
}
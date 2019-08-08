using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Five_Seconds.Services
{
    public interface IMessageBoxService
    {
        void ShowAlert(string title, string message, Action onClosed = null);
        // ...
        Task<string> ShowActionSheet(string title, string cancel, string destruction, string[] buttons = null);
    }
}

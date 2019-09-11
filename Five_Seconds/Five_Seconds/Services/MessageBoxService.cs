using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Five_Seconds.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        private static Page CurrentMainPage { get { return Application.Current.MainPage; } }

        public async void ShowAlert(string title, string message, Action onClosed = null)
        {
            await CurrentMainPage.DisplayAlert(title, message, "확인");
            onClosed?.Invoke();
        }

        public async void ShowConfirm(string title, string message, Action onClosed = null, Action action = null)
        {
            bool confirm = await CurrentMainPage.DisplayAlert(title, message, "확인", "취소");
            if (confirm)
            {
                action?.Invoke();
            }
            else
            {
                onClosed?.Invoke();
            }
        }

        public async Task<string> ShowActionSheet(string title, string cancel, string destruction = null, string[] buttons = null)
        {
            var displayButtons = buttons ?? new string[] { };
            var action = await CurrentMainPage.DisplayActionSheet(title, cancel, destruction, displayButtons);
            return action;
        }
    }
}

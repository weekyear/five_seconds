using Five_Seconds.Models;
using Five_Seconds.Services;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel(INavigation navigation, ILocalData localData) : base(navigation, localData)
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        public ICommand OpenWebCommand { get; }
    }
}
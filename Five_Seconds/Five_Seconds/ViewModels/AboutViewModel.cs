using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Five_Seconds.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel(INavigation navigation) : base(navigation)
        {
            InitAbouts();
            ConstructCommand();

        }


        public List<string> Abouts
        {
            get;
            set;
        }

        public Command<int> ShowAboutCommand { get; set; }

        private void InitAbouts()
        {
            Abouts = new List<string>
            {
                "1) 5초의 법칙 왜 필요한가?",
                "2) 5초의 법칙 왜 효과적인가?",
                "3) 5초의 알람 앱이란?"
            };
        }

        private void ConstructCommand()
        {
            ShowAboutCommand = new Command<int>(async(m) => await ShowAbout(m));
        }

        public async Task ShowAbout(int index)
        {
            switch (index)
            {
                case 0:
                    await Navigation.PushAsync(new AboutDetailPage1());
                    break;
                case 1:
                    await Navigation.PushAsync(new AboutDetailPage2());
                    break;
                case 2:
                    await Navigation.PushAsync(new AboutDetailPage3());
                    break;
            }
        }
    }
}
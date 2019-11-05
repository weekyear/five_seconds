using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
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
            Title = "5초의 법칙이란";

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
                "3) 5초의 알람 차별점"
            };
        }

        private void ConstructCommand()
        {
            ShowAboutCommand = new Command<int>((m) => ShowAbout(m));
        }

        public void ShowAbout(int index)
        {
            string url = string.Empty;

            switch (index)
            {
                case 0:
                    url = "http://blog.daum.net/save_us_222/58";
                    break;
                case 1:
                    url = "http://blog.daum.net/save_us_222/59";
                    break;
                case 2:
                    url = "http://blog.daum.net/save_us_222/60";
                    break;
                case 3:
                    url = "http://blog.daum.net/save_us_222/61";
                    break;
            }

            Device.OpenUri(new Uri(url));
        }
    }
}
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
                "1) 아침에 제시간에 일어나기 위한 방법",
                "2) 우리는 왜 운동하지 않고 공부하지 않는가?",
                "3) 생산적인 사람이 되는 가장 구체적인 방법",
                "4) 불안하거나 공황 증세가 왔을 때 대처하는 법"
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
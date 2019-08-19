using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Five_Seconds.Services;
using Five_Seconds.Views;
using Five_Seconds.Repository;
using Five_Seconds.Models;

namespace Five_Seconds
{
    public partial class App : Application
    {

        public App()
        {
            AdMaiora.RealXaml.Client.AppManager.Init(this);

            DependencyService.Register<ILocalData>();
            DependencyService.Register<IMessageBoxService>();

            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private static ILocalData localData;
        public static ILocalData LocalData
        {
            get
            {
                if (localData == null)
                {
                    localData = new LocalData();
                }
                return localData;
            }
        }
    }
}

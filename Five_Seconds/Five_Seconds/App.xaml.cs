using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Five_Seconds.Services;
using Five_Seconds.Views;
using Five_Seconds.Repository;
using Five_Seconds.Models;
using Xam.Plugin.SimpleAppIntro;
using System.Collections.Generic;
using Xamarin.Essentials;
using SQLite;
using System.ComponentModel;
using Five_Seconds.Helpers;

namespace Five_Seconds
{
    public partial class App : Application
    {
        public static ItemDatabaseGeneric ItemDatabase { get; } = new ItemDatabaseGeneric();
        private bool isNotFirst = Preferences.Get(nameof(isNotFirst), false);

        public App()
        {
            AdMaiora.RealXaml.Client.AppManager.Init(this);

            DependencyService.Register<INavigation>();
            DependencyService.Register<IMissionsRepository>();
            DependencyService.Register<IMissionService>();
            DependencyService.Register<IMessageBoxService>();
            DependencyService.Register<ISpeechToText>();

            InitializeComponent();

            MainPage = new NavigationPage(new MissionsPage());

            if (!isNotFirst)
            {
                var welcomePage = AppIntro.CreateAppIntro();
                MainPage.Navigation.PushModalAsync(welcomePage);
                Preferences.Set(nameof(isNotFirst), true);
            }

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
        

        private static IMissionsRepository missionsRepo;
        public static IMissionsRepository MissionsRepo
        {
            get
            {
                if (missionsRepo == null)
                {
                    missionsRepo = new MissionsRepository();
                }
                return missionsRepo;
            }
        }

        private static IMissionService service;
        public static IMissionService Service
        {
            get
            {
                if (service == null)
                {
                    service = new MissionService(App.MissionsRepo);
                }
                return service;
            }
        }

        private static IAlarmToneRepository alarmToneRepo;
        public static IAlarmToneRepository AlarmToneRepo
        {
            get
            {
                if (alarmToneRepo == null)
                {
                    alarmToneRepo = new AlarmToneRepository();
                }
                return alarmToneRepo;
            }
        }
    }
}

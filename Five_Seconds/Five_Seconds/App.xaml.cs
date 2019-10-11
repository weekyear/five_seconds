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
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Application = Xamarin.Forms.Application;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace Five_Seconds
{
    public partial class App : Application
    {
        public static ItemDatabaseGeneric ItemDatabase { get; } = new ItemDatabaseGeneric(DependencyService.Get<IDatabase>().DBConnect());
        private readonly bool isNotFirst = Preferences.Get(nameof(isNotFirst), false);

        public static bool IsInitFinished;
        public App()
        {
            AdMaiora.RealXaml.Client.AppManager.Init(this);

            DependencyService.Register<INavigation>();
            DependencyService.Register<IAlarmsRepository>();
            DependencyService.Register<IAlarmService>();
            DependencyService.Register<IMessageBoxService>();
            DependencyService.Register<ISpeechToText>();

            InitializeComponent();

            var navigationPage = new NavigationPage(new AlarmsPage());

            navigationPage.On<iOS>().SetPrefersLargeTitles(true);

            MainPage = navigationPage;

            if (!isNotFirst)
            {
                var welcomePage = AppIntro.CreateAppIntro();
                MainPage.Navigation.PushModalAsync(welcomePage);
                Preferences.Set(nameof(isNotFirst), true);
            }

            IsInitFinished = true;
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
        

        private static IAlarmsRepository alarmsRepo;
        public static IAlarmsRepository AlarmsRepo
        {
            get
            {
                if (alarmsRepo == null)
                {
                    alarmsRepo = new AlarmsRepository(ItemDatabase);
                }
                return alarmsRepo;
            }
        }

        private static IAlarmService service;
        public static IAlarmService Service
        {
            get
            {
                if (service == null)
                {
                    service = new AlarmService(AlarmsRepo);
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

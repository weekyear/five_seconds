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
        public static ItemDatabaseGeneric ItemDatabase { get; } = new ItemDatabaseGeneric(DependencyService.Get<IDatabase>().DBConnect());
        private bool isNotFirst = Preferences.Get(nameof(isNotFirst), false);

        public static bool IsInitFinished;
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
        

        private static IMissionsRepository missionsRepo;
        public static IMissionsRepository MissionsRepo
        {
            get
            {
                Console.WriteLine("Get MissionsRepo_App");
                if (missionsRepo == null)
                {
                    Console.WriteLine("Get MissionsRepo_App 1");
                    missionsRepo = new MissionsRepository(ItemDatabase);
                }
                Console.WriteLine("Get MissionsRepo_App 2");
                return missionsRepo;
            }
        }

        private static IMissionService service;
        public static IMissionService Service
        {
            get
            {
                Console.WriteLine("Get Service_App");
                if (service == null)
                {
                    Console.WriteLine("Get Service_App 1");
                    service = new MissionService(MissionsRepo);
                }

                Console.WriteLine("Get Service_App 2");
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

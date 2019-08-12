﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Five_Seconds.Services;
using Five_Seconds.Views;
using Five_Seconds.Repository;

namespace Five_Seconds
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<IMessageBoxService>();

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

        static ILocalData localData;
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

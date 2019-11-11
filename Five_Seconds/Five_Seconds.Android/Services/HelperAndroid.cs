using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using SearchView = Android.Support.V7.Widget.SearchView;
using Five_Seconds.Droid.Services;
using Five_Seconds.Services;
using Plugin.CurrentActivity;
using SQLite;
using Five_Seconds.Repository;

[assembly: Xamarin.Forms.Dependency(typeof(HelperAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class HelperAndroid : IHelper
    {
        public void CollapseSearchView()
        {
            var toolBar = GetToolbar();

            toolBar?.CollapseActionView();
        }

        Toolbar GetToolbar() => CrossCurrentActivity.Current.Activity.FindViewById<Toolbar>(Resource.Id.toolbar);

        public static IAlarmService GetAlarmService()
        {
            IAlarmService alarmService;
            try
            {
                alarmService = App.Service;
            }
            catch (Exception e)
            {
                Console.WriteLine("GetAlarmService_03");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
                alarmService = CreateServiceWithoutCore();
            }

            return alarmService;
        }

        private static AlarmService CreateServiceWithoutCore()
        {
            var deviceStorage = new DeviceStorageAndroid();
            var sqliteConnection = new SQLiteConnection(deviceStorage.GetFilePath("AlarmsSQLite.db3"));
            var itemDatabase = new ItemDatabaseGeneric(sqliteConnection);
            var alarmsRepo = new AlarmsRepository(itemDatabase);
            var service = new AlarmService(alarmsRepo);

            return service;
        }

        public static IAlarmToneRepository GetAlarmToneRepository()
        {
            IAlarmToneRepository alarmToneRepo;
            try
            {
                alarmToneRepo = App.AlarmToneRepo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
                Console.WriteLine("App.ToneRepo == null_NotificationReceiver");
                alarmToneRepo = CreateToneRepoWithoutCore();
            }

            return alarmToneRepo;
        }

        private static AlarmToneRepository CreateToneRepoWithoutCore()
        {
            var deviceStorage = new DeviceStorageAndroid();
            var sqliteConnection = new SQLiteConnection(deviceStorage.GetFilePath("AlarmsSQLite.db3"));
            var itemDatabase = new ItemDatabaseGeneric(sqliteConnection);
            var toneRepo = new AlarmToneRepository(itemDatabase);

            return toneRepo;
        }
    }
}
using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Speech;
using Five_Seconds.Droid.Services;
using Plugin.CurrentActivity;
using Android.Database;
using Android.Provider;
using Android;
using Android.Gms.Ads;
using Xamarin.Forms;
using Five_Seconds.Models;
using Five_Seconds.ViewModels;
using Five_Seconds.Repository;
using SQLite;

namespace Five_Seconds.Droid
{
    [Activity(Label = "@string/FiveSecondAlarm", Icon = "@drawable/ic_five_seconds", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            InitForOpenApp(savedInstanceState);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            SetMobileAds();

            Fabric.Fabric.With(this, new Crashlytics.Crashlytics());
            Crashlytics.Crashlytics.HandleManagedExceptions();

            AlarmHelper.SetComebackNotification(false);
        }

        private void InitForOpenApp(Bundle savedInstanceState)
        {
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
        }

        protected override void OnStart()
        {
            base.OnStart();

            StartApp();
        }

        protected override void OnResume()
        {
            if (SettingToneViewModel.IsFinding)
            {
                SettingToneViewModel.IsFinding = false;
            }
            base.OnResume();
        }

        private void StartApp()
        {
            if (SettingToneViewModel.IsFinding) return;

            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                LoadAppAndRefreshAlarmManager();
                return;
            }

            if (MyPermissions.RequestAudioPermission(this))
            {
                if (MyPermissions.RequestStoragePermission(this))
                {
                    LoadAppAndRefreshAlarmManager();
                }
            }
        }

        private void LoadAppAndRefreshAlarmManager()
        {
            LoadApplication(new App());

            Alarm.IsInitFinished = false;
            var allAlarms = App.AlarmService.GetAllAlarms();
            Alarm.IsInitFinished = true;

            AlarmHelper.RefreshAlarmByManager(allAlarms);
        }

        private void SetMobileAds()
        {
            MobileAds.Initialize(ApplicationContext, GetString(Resource.String.admob_app_id));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (MyPermissions.OnRequestPermissionsResult(this, requestCode, grantResults) && !Alarm.IsInitFinished)
            {
                StartApp();
            }
            else
            {
                MyPermissions.AlertRequestPermissionsWhenDenied(this, requestCode, permissions, grantResults);
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public event Action<string> FileChosen;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == MyPermissions.READ_MEDIA_REQUEST_CODE)
            {
                SettingToneViewModel.IsFinding = true;

                if (resultCode == Result.Ok)
                {
                    if (data == null) return;
                    var _uri = data.Data;
                    var realPath = GetRealPathFromURI(_uri);
                    //var stringUri = data.ToUri(IntentUriType.None);
                    //Uri uri = new Uri(stringUri);

                    FileChosen?.Invoke(realPath);
                }
            }
        }

        private string GetRealPathFromURI(Android.Net.Uri contentURI)
        {
            var documentId = string.Empty;
            ICursor cursor = ContentResolver.Query(contentURI, null, null, null, null);
            if (cursor != null && cursor.MoveToFirst())
            {
                documentId = cursor.GetString(0);
                documentId = documentId.Split(':')[1];
                cursor.Close();
            }


            var path = string.Empty;
            cursor = ContentResolver.Query(
            MediaStore.Audio.Media.ExternalContentUri,
            null, 
            MediaStore.Audio.Media.InterfaceConsts.Id + " = ? ", new[] { documentId }, null);
            if (cursor != null && cursor.MoveToFirst())
            {
                path = cursor.GetString(cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Data));
                cursor.Close();
            }

            return path;
        }
    }
}
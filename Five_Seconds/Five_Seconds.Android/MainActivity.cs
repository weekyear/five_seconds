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

namespace Five_Seconds.Droid
{
    [Activity(Label = "5초의 알람", Icon = "@drawable/ic_five_seconds", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Forms.Init(this, savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            SetMobileAds();
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
            if (MyPermissions.RequestAudioPermission(this) && !SettingToneViewModel.IsFinding)
            {
                LoadApplication(new App());
            }
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
                LoadApplication(new App());
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
                    var stringUri = data.ToUri(IntentUriType.None);
                    Uri uri = new Uri(stringUri);

                    FileChosen?.Invoke(realPath);
                }
            }
            else if (requestCode == MyPermissions.REQUEST_PERMISSION_SETTING)
            {
                if (resultCode == Result.Canceled)
                {
                    SettingToneViewModel.IsFinding = true;
                }
            }
        }

        private string GetRealPathFromURI(Android.Net.Uri contentURI)
        {
            ICursor cursor = ContentResolver.Query(contentURI, null, null, null, null);
            cursor.MoveToFirst();
            string documentId = cursor.GetString(0);
            documentId = documentId.Split(':')[1];
            cursor.Close();

            cursor = ContentResolver.Query(
            MediaStore.Audio.Media.ExternalContentUri,
            null, 
            MediaStore.Audio.Media.InterfaceConsts.Id + " = ? ", new[] { documentId }, null);
            cursor.MoveToFirst();
            string path = cursor.GetString(cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Data));
            cursor.Close();

            return path;
        }
    }
}
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

namespace Five_Seconds.Droid
{
    [Activity(Label = "5초의 알람", Icon = "@drawable/ic_five_seconds", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int MY_PERMISSIONS_REQUEST_READ_MEDIA = 2356;
        static readonly int READ_REQUEST_CODE = 42;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Forms.Init(this, savedInstanceState);
            XamEffects.Droid.Effects.Init();
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            SetMobileAds();

            LoadApplication(new App());
        }
        
        private void SetMobileAds()
        {
            MobileAds.Initialize(ApplicationContext, GetString(Resource.String.admob_app_id));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            switch (requestCode)
            {
                case MY_PERMISSIONS_REQUEST_READ_MEDIA:
                    if ((grantResults.Length > 0) && (grantResults[0] == Permission.Granted))
                    {
                        Intent intent = new Intent(Intent.ActionOpenDocument);

                        intent.AddCategory(Intent.CategoryOpenable);
                        intent.SetType("audio/*");

                        StartActivityForResult(intent, READ_REQUEST_CODE);
                    }
                    else
                    {
                        return;
                    }
                    break;

                default:
                    break;
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void RequestReadExternalStoragePermission()
        {
            if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Permission.Granted)
            {
                // Should we show an explanation?
                if (ShouldShowRequestPermissionRationale(Manifest.Permission.ReadExternalStorage))
                {
                    // Explain to the user why we need to read the contacts
                }

                RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, MY_PERMISSIONS_REQUEST_READ_MEDIA);

                // MY_PERMISSIONS_REQUEST_READ_EXTERNAL_STORAGE is an
                // app-defined int constant that should be quite unique

                return;
            }
            else
            {
                Intent intent = new Intent(Intent.ActionOpenDocument);

                intent.AddCategory(Intent.CategoryOpenable);
                intent.SetType("audio/*");

                StartActivityForResult(intent, READ_REQUEST_CODE);
            }
        }

        public event Action<string> FileChosen;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 42 && resultCode == Result.Ok)
            {
                if (data == null) return;
                var _uri = data.Data;
                var realPath = GetRealPathFromURI(_uri);
                var stringUri = data.ToUri(IntentUriType.None);
                Uri uri = new Uri(stringUri);

                FileChosen?.Invoke(realPath);
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
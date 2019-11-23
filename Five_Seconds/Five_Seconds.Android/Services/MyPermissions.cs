using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Five_Seconds.ViewModels;
using Plugin.CurrentActivity;

namespace Five_Seconds.Droid.Services
{
    public static class MyPermissions
    {
        const int MY_PERMISSIONS_REQUEST_FILE_STORAGE = 2356;
        const int MY_PERMISSIONS_RECORD_AUDIO = 2357;
        public static readonly int READ_MEDIA_REQUEST_CODE = 42;
        public static readonly int REQUEST_PERMISSION_SETTING = 43;

        public static bool OpenExternalStorage(Activity activity)
        {
            if (activity.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Permission.Granted)
            {
                // Should we show an explanation?
                if (activity.ShouldShowRequestPermissionRationale(Manifest.Permission.ReadExternalStorage))
                {
                    // Explain to the user why we need to read the contacts
                }

                activity.RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, MY_PERMISSIONS_REQUEST_FILE_STORAGE);

                // MY_PERMISSIONS_REQUEST_READ_EXTERNAL_STORAGE is an
                // app-defined int constant that should be quite unique

                return false;
            }
            else
            {
                Intent intent = new Intent(Intent.ActionOpenDocument);

                intent.AddCategory(Intent.CategoryOpenable);
                intent.SetType("audio/*");

                activity.StartActivityForResult(intent, READ_MEDIA_REQUEST_CODE);

                return true;
            }
        }

        public static bool RequestStoragePermission(Activity activity)
        {
            if (activity.CheckCallingOrSelfPermission(Manifest.Permission.ReadExternalStorage) != Permission.Granted)
            {
                // Should we show an explanation?
                if (activity.ShouldShowRequestPermissionRationale(Manifest.Permission.ReadExternalStorage))
                {
                    // Explain to the user why we need to read the contacts
                }

                activity.RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, MY_PERMISSIONS_REQUEST_FILE_STORAGE);

                // MY_PERMISSIONS_REQUEST_READ_EXTERNAL_STORAGE is an
                // app-defined int constant that should be quite unique

                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool RequestAudioPermission(Activity activity)
        {
            if (activity.CheckCallingOrSelfPermission(Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                // Should we show an explanation?
                if (activity.ShouldShowRequestPermissionRationale(Manifest.Permission.RecordAudio))
                {
                    // Explain to the user why we need to read the contacts
                }

                activity.RequestPermissions(new string[] { Manifest.Permission.RecordAudio }, MY_PERMISSIONS_RECORD_AUDIO);

                // MY_PERMISSIONS_REQUEST_READ_EXTERNAL_STORAGE is an
                // app-defined int constant that should be quite unique

                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool OnRequestPermissionsResult(Activity activity, int requestCode, [GeneratedEnum] Permission[] grantResults)
        {
            if (grantResults.Length == 0) return true;
            switch (requestCode)
            {
                case MY_PERMISSIONS_REQUEST_FILE_STORAGE:
                    if (grantResults?[0] != Permission.Granted)
                    {
                        return false;
                    }

                    if (grantResults.Length > 0)
                    {
                        if (SettingToneViewModel.IsFinding)
                        {
                            Intent intent = new Intent(Intent.ActionOpenDocument);

                            intent.AddCategory(Intent.CategoryOpenable);
                            intent.SetType("audio/*");

                            activity.StartActivityForResult(intent, READ_MEDIA_REQUEST_CODE);
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case MY_PERMISSIONS_RECORD_AUDIO:
                    if (grantResults?[0] != Permission.Granted)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                default:
                    return false;
            }
        }

        public static void AlertRequestPermissionsWhenDenied(Activity activity, int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case MY_PERMISSIONS_REQUEST_FILE_STORAGE:
                    AlertFileStoragePermissions(activity, permissions, grantResults);
                    break;
                case MY_PERMISSIONS_RECORD_AUDIO:
                    AlertAudioPermissions(activity, permissions, grantResults);
                    break;
            }
        }


        private static void AlertAudioPermissions(Activity activity, string[] permissions, Permission[] grantResults)
        {
            for (int i = 0; i < permissions.Length; i++)
            {
                var permission = permissions[i];
                //var showRationale = ShouldShowRequestPermissionRationale(permission);
                if (grantResults[i] == Permission.Denied)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(activity)
                        .SetTitle(Application.Context.GetString(Resource.String.RequestMicPermission))
                        .SetMessage(Application.Context.GetString(Resource.String.ActivatePermission))
                        .SetPositiveButton(Application.Context.GetString(Resource.String.Settings), (senderAlert, args) =>
                        {
                            var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                            var uri = Android.Net.Uri.FromParts("package", activity.PackageName, null);
                            intent.SetData(uri);
                            activity.StartActivityForResult(intent, REQUEST_PERMISSION_SETTING);
                        })
                        .SetNegativeButton(Application.Context.GetString(Resource.String.Close), (senderAlert, args) =>
                        {
                            activity.Finish();
                        })
                        .SetCancelable(false);

                    if (!CrossCurrentActivity.Current.Activity.IsFinishing)
                    {
                        alert.Show();
                    }
                }
            }
        }
        private static void AlertFileStoragePermissions(Activity activity, string[] permissions, Permission[] grantResults)
        {
            for (int i = 0; i < permissions.Length; i++)
            {
                var permission = permissions[i];
                if (grantResults[i] == Permission.Denied)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(activity)
                        .SetTitle(Application.Context.GetString(Resource.String.RequestStoragePermission))
                        .SetMessage(Application.Context.GetString(Resource.String.ActivatePermission))
                        .SetPositiveButton(Application.Context.GetString(Resource.String.Settings), (senderAlert, args) =>
                        {
                            SettingToneViewModel.IsFinding = true;
                            var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                            var uri = Android.Net.Uri.FromParts("package", activity.PackageName, null);
                            intent.SetData(uri);
                            activity.StartActivityForResult(intent, REQUEST_PERMISSION_SETTING);
                        })
                        .SetNegativeButton(Application.Context.GetString(Resource.String.Close), (senderAlert, args) =>
                        {
                        })
                        .SetCancelable(false);

                    if (!CrossCurrentActivity.Current.Activity.IsFinishing)
                    {
                        alert.Show();
                    }
                }
            }
        }
    }
}
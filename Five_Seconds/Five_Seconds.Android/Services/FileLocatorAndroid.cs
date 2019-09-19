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
using Five_Seconds.Droid.Services;
using Five_Seconds.Services;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Xamarin.Forms.Dependency(typeof(FileLocatorAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class FileLocatorAndroid : IFileLocator
    {
        MainActivity _mainActivity;

        public void OpenFileLocator()
        {
            var activity = CrossCurrentActivity.Current.Activity;

            _mainActivity = (MainActivity)activity;

            _mainActivity.FileChosen += OnFileChosen;

            _mainActivity.RequestReadExternalStoragePermission();
        }

        public event Action<string> FileChosen;

        void OnFileChosen(string path)
        {
            FileChosen?.Invoke(path);
            _mainActivity.FileChosen -= OnFileChosen;
        }
    }
}
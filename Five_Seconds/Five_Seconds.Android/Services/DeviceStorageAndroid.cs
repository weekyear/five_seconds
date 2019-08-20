﻿using System;
using System.Collections.Generic;
using System.IO;
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

[assembly: Xamarin.Forms.Dependency(typeof(DeviceStorageAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class DeviceStorageAndroid : IDeviceStorageService
    {
        public string GetFilePath(string fileName)
        {
            return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
        }
    }
}
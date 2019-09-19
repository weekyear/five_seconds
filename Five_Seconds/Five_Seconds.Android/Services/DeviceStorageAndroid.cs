using System;
using System.IO;
using Five_Seconds.Droid.Services;
using Five_Seconds.Services;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceStorageAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class DeviceStorageAndroid : IDeviceStorageService
    {
        public string GetFilePath(string fileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
        }
    }
}
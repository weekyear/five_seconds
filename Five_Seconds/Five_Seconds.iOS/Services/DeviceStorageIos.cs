using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Five_Seconds.iOS.Services;
using Five_Seconds.Services;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceStorageIos))]
namespace Five_Seconds.iOS.Services
{
    public class DeviceStorageIos : IDeviceStorageService
    {
        public string GetFilePath(string fileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", fileName);
        }
    }
}
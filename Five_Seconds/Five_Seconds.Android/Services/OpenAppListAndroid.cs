using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Services;

[assembly: Xamarin.Forms.Dependency(typeof(OpenAppListAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class OpenAppListAndroid : IOpenAppList
    {
        public List<AppPackage> OpenAppList()
        {
            Intent mainIntent = new Intent(Intent.ActionMain, null);
            mainIntent.AddCategory(Intent.CategoryLauncher);
            List<ResolveInfo> pkgAppsList = new List<ResolveInfo>(Application.Context.PackageManager.QueryIntentActivities(mainIntent, 0));

            var appPkgList = new List<AppPackage>();
            foreach (var pkgApp in pkgAppsList)
            {
                appPkgList.Add(new AppPackage()
                {
                    Label = pkgApp.LoadLabel(Application.Context.PackageManager),
                    PackageName = pkgApp.ActivityInfo.ApplicationInfo.PackageName
                });
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.WriteLine($"ResolvePackageName : {pkgApp.ResolvePackageName}");
                Console.WriteLine($"PackageName : {pkgApp.ActivityInfo.ApplicationInfo.PackageName}");
                Console.WriteLine($"ProviderInfo : {pkgApp.ProviderInfo}");
                Console.WriteLine($"IconResource : {pkgApp.IconResource}");
                Console.WriteLine($"Label : {pkgApp.LoadLabel(Application.Context.PackageManager)}");
            }

            appPkgList = appPkgList.OrderBy(p => p.Label).ToList();
            return appPkgList;
        }
    }
}
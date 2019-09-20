using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Five_Seconds.Services;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(Database))]
namespace Five_Seconds.Services
{
    public class Database : IDatabase
    {
        public SQLiteConnection DBConnect()
        {
            if (Device.RuntimePlatform != "Test")
            {
                string path = DependencyService.Get<IDeviceStorageService>().GetFilePath("AlarmsSQLite.db3");
                return CreateConnection(path);
            }
            else
            {
                return null;
            }
        }

        private SQLiteConnection CreateConnection(string path)
        {
            return new SQLiteConnection(path);
        }
    }
}

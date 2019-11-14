using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Five_Seconds.Services;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(Database))]
namespace Five_Seconds.Services
{
    public class Database : IDatabase
    {
        public SQLiteConnection DBConnect()
        
        {
            //if (Device.RuntimePlatform != "Test")
            //{
            //    string path = DependencyService.Get<IDeviceStorageService>().GetFilePath("AlarmsSQLite.db3");
            //    return CreateConnection(path);
            //}

            //TODO::지금은 안드로이드 전용 Path만 설정해놨음. 나중에 방안 찾아야 함 꼭

            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "AlarmsSQLite.db3");
                return CreateConnection(path);
            }
            catch
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", "AlarmsSQLite.db3");
                return CreateConnection(path);
            }
        }

        private SQLiteConnection CreateConnection(string path)
        {
            return new SQLiteConnection(path);
        }
    }
}

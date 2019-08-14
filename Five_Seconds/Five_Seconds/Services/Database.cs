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
        private SQLiteConnection connection;
        public SQLiteConnection DBConnect()
        {
            switch (Device.RuntimePlatform)
            {
                case "Android":
                    CreateConnection_Android();
                    break;
                case "iOS":
                    CreateConnection_iOS();
                    break;
                case "Test":
                    break;
            }
            return connection;
        }

        private void CreateConnection_Android()
        {
            var fileName = "MissionsSQLite.db3";
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(folder, fileName);
            connection = new SQLiteConnection(path);
        }

        private void CreateConnection_iOS()
        {
            var fileName_iOS = "MissionsSQLite.db3";
            string folder_iOS = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryFolder = Path.Combine(folder_iOS, "..", "Library");
            var path_iOS = Path.Combine(libraryFolder, fileName_iOS);
            connection = new SQLiteConnection(path_iOS);
        }
    }
}

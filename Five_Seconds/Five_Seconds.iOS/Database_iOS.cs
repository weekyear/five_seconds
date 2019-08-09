using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Five_Seconds.iOS;
using Five_Seconds.Services;
using Foundation;
using SQLite;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(Database_iOS))]
namespace Five_Seconds.iOS
{
    public class Database_iOS : IDatabase
    {
        public Database_iOS() { }
        public SQLiteConnection DBConnect()
        {
            var fileName = "MissionsSQLite.db3";
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryFolder = Path.Combine(folder, "..", "Library");
            var path = Path.Combine(libraryFolder, fileName);
            var connection = new SQLiteConnection(path);
            return connection;
        }
    }
}
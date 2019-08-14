using System.IO;
using Five_Seconds.Droid;
using Five_Seconds.Services;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(Database_Android))]
namespace Five_Seconds.Droid
{
    public class Database_Android : IDatabase
    {
        public Database_Android() { }
        public SQLiteConnection DBConnect()
        {
            var fileName = "MissionsSQLite.db3";
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(folder, fileName);
            var connection = new SQLiteConnection(path);
            return connection;
        }
    }
}
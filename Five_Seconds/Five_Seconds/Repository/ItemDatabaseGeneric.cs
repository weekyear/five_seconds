using Five_Seconds.Models;
using Five_Seconds.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Repository
{
    public class ItemDatabaseGeneric
    {
        static object locker = new object();
        SQLiteConnection connection;

        public ItemDatabaseGeneric(SQLiteConnection connection)
        {
            Console.WriteLine("Constructor_ItemDatabaseGeneric");
            this.connection = connection;
            Console.WriteLine($"connection is null? : {connection == null}");
            Console.WriteLine("After_DBConnect");
            connection.CreateTable<Mission>();
            connection.CreateTable<Alarm>();
            connection.CreateTable<Record>();
            connection.CreateTable<DaysOfWeek>();
            connection.CreateTable<AlarmTone>();
            Console.WriteLine("After_CreateTable");
        }

        public ItemDatabaseGeneric(string path)
        {
            Console.WriteLine("Constructor_ItemDatabaseGeneric");
            connection = new SQLiteConnection(path);
            Console.WriteLine($"connection is null? : {connection == null}");
            Console.WriteLine($"connection is null? : {connection.DatabasePath}");
            Console.WriteLine("After_DBConnect");
            var resultMissions = connection.CreateTable<Mission>();
            Console.WriteLine($"Mission Table is null? : {resultMissions.ToString()}");
            var resultAlarms = connection.CreateTable<Alarm>();
            Console.WriteLine($"Alarm Table is null? : {resultAlarms.ToString()}");
            var resultRecords = connection.CreateTable<Record>();
            Console.WriteLine($"Record Table is null? : {resultRecords.ToString()}");
            var resultDaysOfWeeks = connection.CreateTable<DaysOfWeek>();
            Console.WriteLine($"DaysOfWeek Table is null? : {resultDaysOfWeeks.ToString()}");
            var resultAlarmTones = connection.CreateTable<AlarmTone>();
            Console.WriteLine($"AlarmTone Table is null? : {resultAlarmTones.ToString()}");

            Console.WriteLine("After_CreateTable");
        }


        public IEnumerable<T> GetObjects<T>() where T : IObject, new()
        {
            lock (locker)
            {
                return (from i in connection.Table<T>() select i).ToList();
            }
        }
        public IEnumerable<T> GetFirstObjects<T>() where T : IObject, new()
        {
            lock (locker)
            {
                return connection.Query<T>($"SELECT * FROM {nameof(T)} WHERE Name = 'First'");
            }
        }

        public T GetObject<T>(int id) where T : IObject, new()
        {
            lock (locker)
            {
                Console.WriteLine("GetObject");
                var mission = connection.Table<T>().Where(x => x.Id == id).FirstOrDefault();
                Console.WriteLine($"mission is null? : {mission == null}");
                return mission;
            }
        }
        public int SaveObject<T>(T obj) where T : IObject
        {
            lock (locker)
            {
                if (obj.Id != 0)
                {
                    connection.Update(obj);
                }
                else
                {
                    connection.Insert(obj);
                }
                return obj.Id;
            }
        }
        public int DeleteObject<T>(int id) where T : IObject, new()
        {
            lock (locker)
            {
                return connection.Delete<T>(id);
            }
        }
        public void DeleteAllObjects<T>()
        {
            lock (locker)
            {
                connection.DropTable<T>();
                connection.CreateTable<T>();
            }
        }
    }
}

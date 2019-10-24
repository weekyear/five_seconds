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
        static readonly object locker = new object();
        readonly SQLiteConnection connection;

        public ItemDatabaseGeneric(SQLiteConnection connection)
        {
            this.connection = connection;
            connection.CreateTable<Alarm>();
            connection.CreateTable<DaysOfWeek>();
            connection.CreateTable<AlarmTone>();
            connection.CreateTable<Record>();
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
                return connection.Table<T>().Where(x => x.Id == id).FirstOrDefault();
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

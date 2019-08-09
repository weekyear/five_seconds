using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Services
{
    public interface IDatabase
    {
        SQLiteConnection DBConnect();
    }
}

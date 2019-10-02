using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Models
{
    [Table("Trace")]
    public class Trace : IObject
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }

        public int AlarmId { get; }
    }
}

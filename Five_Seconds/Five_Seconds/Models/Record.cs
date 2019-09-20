using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Models
{
    [Table("Records")]
    public class Record : IObject
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Alarm))]
        public int AlarmId { get; set; }

        public DateTime Date { get; set; }
        public int RecordTime { get; set; }
        public bool IsSuccess { get; set; }
    }
}

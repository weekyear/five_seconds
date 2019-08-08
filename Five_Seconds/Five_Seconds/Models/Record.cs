using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Models
{
    public class Record
    {
        public DateTime Date { get; set; }
        public int RecordTime { get; set; }
        public bool IsSuccess { get; set; }
    }
}

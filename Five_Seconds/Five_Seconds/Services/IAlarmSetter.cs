using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Services
{
    public interface IAlarmSetter
    {
        void SetAlarm(Alarm alarm);

        void DeleteAlarm(int id);

        void DeleteAllAlarms(IEnumerable<Alarm> alarms);
    }
}

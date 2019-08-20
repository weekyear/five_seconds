using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Services
{
    public interface IAlarmSetter
    {
        void SetAlarm(Alarm alarm);

        void SetRepeatingAlarm(Alarm alarm);

        void DeleteAlarm(Alarm alarm);

        void DeleteAllAlarms(List<Alarm> alarms);
    }
}

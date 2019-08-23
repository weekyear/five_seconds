using Five_Seconds.Models;
using Five_Seconds.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Repository
{
    public interface IAlarmToneRepository
    {
        ItemDatabaseGeneric ItemDatabase { get; }

        //Settings GetSettings();
        void SetDefaultTones();
        IEnumerable<AlarmTone> GetAllTones();
        int AddTone(AlarmTone alarmTone);
        int DeleteTone(AlarmTone alarmTone);
        AlarmTone GetTone(int id);
    }
}

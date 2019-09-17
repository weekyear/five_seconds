using Five_Seconds.Helpers;
using Five_Seconds.Services;
using Five_Seconds.Views;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Five_Seconds.Models
{
    [Table("Mission")]
    public class Mission : INotifyPropertyChanged, IObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public double Percentage { get; set; }

        //private bool isActive = true;
        //public bool IsActive
        //{
        //    get
        //    {
        //        return isActive;
        //    }
        //    set
        //    {
        //        if (isActive == value) return;
        //        isActive = value;
        //        App.Service.SaveMissionAtLocal(this);
        //    }
        //}

        public bool IsActive { get; set; } = true;

        public void OnIsActiveChanged()
        {
            if (App.IsInitFinished)
            {
                if (!DaysOfWeek.GetHasADayBeenSelected(Alarm.Days) && IsActive && Alarm.TimeOffset.Subtract(DateTimeOffset.Now).Ticks < 0)
                {
                    if (Alarm.Time.Subtract(DateTime.Now.TimeOfDay).Ticks < 0)
                    {
                        Alarm.Date = DateTime.Now.Date.AddDays(1);
                    }
                    else
                    {
                        Alarm.Date = DateTime.Now.Date;
                    }

                    App.Service.SaveMission(this);
                }
                else
                {
                    App.Service.SaveMissionAtLocal(this);
                }

                if (IsActive)
                {
                    var diffString = CreateDateString.CreateTimeRemainingString(Alarm.NextAlarmTime);
                    DependencyService.Get<IToastService>().Show(diffString);
                }
            }
        }

        [OneToMany]
        public List<Record> Records { get; set; } = new List<Record>();

        [OneToOne]
        public Alarm Alarm { get; set; } = new Alarm();
        public int AlarmId { get; set; }

        public Mission() { }

        public Mission (Mission original)
        {
            Id = original.Id;
            Name = original.Name;
            Percentage = original.Percentage;
            IsActive = original.IsActive;
            Alarm = new Alarm(original.Alarm);
            AlarmId = original.AlarmId;
        }
    }
}
using Five_Seconds.Helpers;
using Five_Seconds.Services;
using SQLite;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Five_Seconds.Models
{
    public partial class Alarm : INotifyPropertyChanged, IObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static bool IsInitFinished = false;

        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public double Percentage { get; set; }

        public bool IsActive { get; set; } = true;

        public void OnIsActiveChanged()
        {
            if (IsInitFinished)
            {
                if (IsActive)
                {
                    if (!DaysOfWeek.GetHasADayBeenSelected(Days) && TimeOffset.Subtract(DateTimeOffset.Now).Ticks < 0)
                    {
                        if (Time.Subtract(DateTime.Now.TimeOfDay).Ticks < 0)
                        {
                            Date = DateTime.Now.Date.AddDays(1);
                        }
                        else
                        {
                            Date = DateTime.Now.Date;
                        }

                    }

                    App.Service.SaveAlarm(this);
                    var diffString = CreateDateString.CreateTimeRemainingString(NextAlarmTime);
                    DependencyService.Get<IToastService>().Show(diffString);
                }
                else
                {
                    App.Service.TurnOffAlarm(this);
                }
            }
        }

        public static void ChangeIsActive(Alarm alarm, bool isActive)
        {
            IsInitFinished = false;
            alarm.IsActive = isActive;
            IsInitFinished = true;
        }

        public bool IsSelected { get; set; } = false;

        public Alarm() { }

        public Alarm(Alarm original)
        {
            Id = original.Id;
            Name = original.Name;
            Percentage = original.Percentage;
            IsActive = original.IsActive;
            IsAlarmOn = original.IsAlarmOn;
            Volume = original.Volume;
            IsVibrateOn = original.IsVibrateOn;
            TimeOffset = original.TimeOffset;

            Days = new DaysOfWeek(original.Days);
            DaysId = original.DaysId;
            Tone = original.Tone;
        }
    }
}
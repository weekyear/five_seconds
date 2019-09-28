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

        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public double Percentage { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsSuccess { get; set; } = false;

        public void OnIsActiveChanged()
        {
            if (App.IsInitFinished)
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
            IsCountOn = original.IsCountOn;
            TimeOffset = original.TimeOffset;

            Days = new DaysOfWeek(original.Days);
            DaysId = original.DaysId;
            Tone = original.Tone;
        }
    }
}
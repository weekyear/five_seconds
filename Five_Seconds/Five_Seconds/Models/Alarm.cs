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

        public string Name { get; set; } = string.Empty;
        public double Percentage { get; set; }

        public bool IsVoiceRecognition { get; private set; } = true;
        public bool IsNotDelayAlarm { get; private set; } = false;
        public int AlarmType
        {
            get
            {
                if (IsVoiceRecognition)
                {
                    if (IsNotDelayAlarm)
                    {
                        return 2;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                switch (value)
                {
                    case 0:
                        IsVoiceRecognition = false;
                        IsNotDelayAlarm = false;
                        break;
                    case 1:
                        IsVoiceRecognition = true;
                        IsNotDelayAlarm = false;
                        break;
                    case 2:
                        IsVoiceRecognition = true;
                        IsNotDelayAlarm = true;
                        break;
                }
            }
        }

        public bool IsFiveCount  { get; set; } = true;

        public bool IsActive { get; set; } = true;
        public void OnIsActiveChanged()
        {
            try
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
            catch
            {
                Console.WriteLine("OnIsActiveChangedException");
            }
        }

        public static void ChangeIsActive(Alarm alarm, bool isActive)
        {
            IsInitFinished = false;
            alarm.IsActive = isActive;
            IsInitFinished = true;
        }

        public bool IsSelected { get; set; } = false;
        public bool HasWakeUpText { get; set; } = false;
        public string WakeUpText { get; set; }

        public Alarm() { }

        public Alarm(Alarm original)
        {
            Id = original.Id;
            Name = original.Name;
            Percentage = original.Percentage;
            AlarmType = original.AlarmType;
            ChangeIsActive(this, original.IsActive);
            IsAlarmOn = original.IsAlarmOn;
            IsVibrateOn = original.IsVibrateOn;
            IsVoiceRecognition = original.IsVoiceRecognition;
            IsFiveCount = original.IsFiveCount;
            IsNotDelayAlarm = original.IsNotDelayAlarm;
            HasWakeUpText = original.HasWakeUpText;
            WakeUpText = original.WakeUpText;

            Volume = original.Volume;
            TimeOffset = original.TimeOffset;

            Days = new DaysOfWeek(original.Days);
            DaysId = original.DaysId;
            Tone = original.Tone;
        }
    }
}
using Five_Seconds.Models;
using Five_Seconds.Repository;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class MissionViewModel : BaseViewModel
    {
        public MissionViewModel(INavigation navigation, IMissionsRepository missionRepo) : base(navigation, missionRepo)
        {
            Mission = new Mission();

            ConstructCommand();
        }

        public MissionViewModel(INavigation navigation, IMissionsRepository missionRepo, Mission mission) : base(navigation, missionRepo)
        {
            Mission = new Mission(mission);

            ConstructCommand();
        }

        private void ConstructCommand()
        {
            CloseCommand = new Command(async () => await ClosePopup());
            SaveCommand = new Command(async () => await Save());
        }

        // Command
        public Command CloseCommand { get; set; }
        public Command SaveCommand { get; set; }

        // Property
        public Mission Mission
        {
            get; set;
        }

        public string Name
        {
            get { return Mission.Name; }
            set
            {
                if (Mission.Name == value) return;
                Mission.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public Alarm Alarm
        {
            get { return Mission.Alarm; }
            set
            {
                if (Mission.Alarm == value) return;
                Mission.Alarm = value;
                OnPropertyChanged(nameof(Alarm));
            }
        }

        public TimeSpan Time
        {
            get
            {
                if (Mission.Alarm.Time == TimeSpan.Zero)
                {
                    Mission.Alarm.Time = DateTime.Now.TimeOfDay;
                }
                return Mission.Alarm.Time;
            }
            set
            {
                if (Mission.Alarm.Time == value) return;
                Mission.Alarm.Time = value;
                OnPropertyChanged(nameof(Time));
            }
        }
        public DaysOfWeek Days
        {
            get { return Mission.Alarm.Days; }
            set
            {
                if (Mission.Alarm.Days == value) return;
                Mission.Alarm.Days = value;
                OnPropertyChanged(nameof(Days));
            }
        }

        public bool IsAlarmOn
        {
            get { return Mission.Alarm.IsAlarmOn; }
            set
            {
                if (Mission.Alarm.IsAlarmOn == value) return;
                Mission.Alarm.IsAlarmOn = value;
                OnPropertyChanged(nameof(IsAlarmOn));
            }
        }

        public int Volume
        {
            get { return Mission.Alarm.Volume; }
            set
            {
                if (Mission.Alarm.Volume == value) return;
                Mission.Alarm.Volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        public bool IsVibrateOn
        {
            get { return Mission.Alarm.IsVibrateOn; }
            set
            {
                if (Mission.Alarm.IsVibrateOn == value) return;
                Mission.Alarm.IsVibrateOn = value;
                OnPropertyChanged(nameof(IsVibrateOn));
            }
        }

        public int VibeFrequency
        {
            get { return Mission.Alarm.VibeFrequency; }
            set
            {
                if (Mission.Alarm.VibeFrequency == value) return;
                Mission.Alarm.VibeFrequency = value;
                OnPropertyChanged(nameof(VibeFrequency));
            }
        }

        // Validation
        public bool HasDayBeenSelected { get; set; } = true;

        // Methods

        private async Task ClosePopup()
        {
            await Navigation.PopAsync(true);
        }
        private async Task Save()
        {
            MissionRepo.SaveMission(Mission);
            await ClosePopup();
        }

        public static bool AreEqual<T>(T left, T right)
        {
            if (left == null)
                return right == null;
            if (left is IComparable<T>)
            {
                IComparable<T> lval = left as IComparable<T>;
                if (right is IComparable<T>)
                    return lval.CompareTo(right) == 0;
                else
                    throw new ArgumentException("Type does not implement IComparable<T>", nameof(right));
            }
            else // 실패
            {
                throw new ArgumentException("Type does not implement IComparable<T>", nameof(left));
            }
        }
    }
}

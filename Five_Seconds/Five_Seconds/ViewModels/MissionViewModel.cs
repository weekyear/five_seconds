using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Five_Seconds.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class MissionViewModel : BaseViewModel
    {
        public MissionViewModel(INavigation navigation) : base(navigation)
        {
            Mission = new Mission();

            ConstructCommand();
        }

        public MissionViewModel(INavigation navigation, Mission mission) : base(navigation)
        {
            Mission = new Mission(mission);

            ConstructCommand();
        }

        private void ConstructCommand()
        {
            CloseCommand = new Command(async () => await ClosePopup());
            SaveCommand = new Command(async () => await Save());
            ShowSettingToneCommand = new Command(async () => await ShowSettingTone());
        }

        // Command
        public Command CloseCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command ShowSettingToneCommand { get; set; }

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

        private DateTime dateTime = DateTime.Now;
        public DateTime Date
        {
            get
            {
                return dateTime;
            }
            set
            {
                if (dateTime == value) return;
                dateTime = value;
                OnPropertyChanged(nameof(Date));
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
            get { return Alarm.Days; }
            set
            {
                if (Alarm.Days == value) return;
                Alarm.Days = value;
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
            if (string.IsNullOrEmpty(Name))
            {
                await Application.Current.MainPage.DisplayAlert("", "미션 이름을 깜빡하셨어요!", "확인");
            }
            else
            {
                Service.SaveMission(Mission);
                await ClosePopup();
            }
        }

        private async Task ShowSettingTone()
        {
            await Navigation.PushAsync(new SettingTonePage(Navigation, Mission));
        }
    }
}

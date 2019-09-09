using Five_Seconds.CustomControls;
using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Five_Seconds.Views;
using System;
using System.Text;
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

            SubscribeMessage();

            ConstructCommand();
        }

        public MissionViewModel(INavigation navigation, Mission mission) : base(navigation)
        {
            Mission = new Mission(mission);

            SubscribeMessage();

            ConstructCommand();
        }

        private void SubscribeMessage()
        {
            MessagingCenter.Subscribe<DaysOfWeekSelectionView>(this, "dayOfWeek_Clicked", (sender) =>
            {
                Date = SetMinimumDate();
                Alarm.IsToday = true;
                OnPropertyChanged(nameof(DateString));
            });
        }

        private DateTime SetMinimumDate()
        {
            if (Time.Subtract(DateTime.Now.TimeOfDay).Ticks < 0)
            {
                return DateTime.Now.AddDays(1).Date;
            }
            else
            {
                return DateTime.Now.Date;
            }
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

        public DateTime Date
        {
            get
            {
                return Alarm.Date;
            }
            set
            {
                if (Alarm.Date == value) return;
                Alarm.Date = value;
                OnPropertyChanged(nameof(DateString));
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
                DateToStringWhenTimeChanged();
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

        public string DateString
        {
            get
            {
                return Alarm.DateString;
            }
        }

        private void DateToStringWhenTimeChanged()
        {
            if (Alarm.IsToday && Time.Subtract(DateTime.Now.TimeOfDay).Ticks < 0)
            {
                Date = DateTime.Now.AddDays(1).Date;
            }
            else if (Alarm.IsToday && Time.Subtract(DateTime.Now.TimeOfDay).Ticks > 0)
            {
                Date = DateTime.Now.Date;
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
            else if (CalculateNextAlarmTime.NextAlarmTime(Alarm).Subtract(DateTime.Now).Ticks < 0)
            {
                await Application.Current.MainPage.DisplayAlert("", "이미 지난 시간으로 설정하셨어요!", "확인");
            }
            else
            {
                Mission.IsActive = true;
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
